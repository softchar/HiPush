using Hi.Infrastructure.Base;
using Hi.NetWork.Buffer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hi.NetWork.Protocols;

namespace Hi.NetWork.Socketing
{
    /// <summary>
    /// 发送流
    /// </summary>
    public class ChannelReceivingStream
    {
        int bufferSize;
        ByteBlock block;

        IByteBuffer pool;
        IFramer framer;

        ChannelReceiving receiving;
        ChannelReceivingBuffer receiveBuffer;

        public ChannelReceivingStream(Socket socket, IByteBuffer pool, IFramer framer, int bufferSize)
        {
            this.bufferSize = bufferSize;

            this.pool = pool;
            this.framer = framer;

            this.receiveBuffer = new ChannelReceivingBuffer(pool);
            this.receiving = new ChannelReceiving(socket);
            this.receiving.ReceiveCompleted = receiveCompleted;
        }

        /// <summary>
        /// 接收接收
        /// </summary>
        public void StartReceiving()
        {
            tryReceiving();
        }

        private void tryReceiving()
        {
            if (receiving.IsClose) return;
            if (receiving.IsReceiving) return;

            var block = receiveBuffer.GetBlock();
            receive(block);

        }

        private void receiveCompleted(ByteBlock block)
        {
            Ensure.IsNotNull(block, "block不能为空");

            receiveBuffer.QueueReceving(block);

            receiving.ExitReceiving();

            framerParse();

            tryReceiving();
        }

        private void framerParse()
        {
            if (isFramering()) return;

            var blocks = receiveBuffer.GetReceivedBlocks();

            if (blocks != null && blocks.Count() > 0)
            {
                framer.Unpacking(blocks);
                blocks.ToList().ForEach(blk => 
                {
                    pool.Return(blk);
                });
                
            }

            exitFramering();
        }

        private void receive(ByteBlock block)
        {
            receiving.Receive(block);
        }

        int isframeringMark = 0;
        private bool isFramering()
        {
            return Interlocked.CompareExchange(ref isframeringMark, 1, 0) == 1;
        }

        private void exitFramering()
        {
            Interlocked.Exchange(ref isframeringMark, 0);
        }


    }

    /// <summary>
    /// 发送,主要负责发送机制
    /// </summary>
    internal class ChannelReceiving
    {
        private Socket socket;
        private SocketAsyncEventArgs receiveSocket;
        private int receivingMark;
        private bool isClose;
        private Action<ByteBlock> receiveCompleted;

        /// <summary>
        /// 是否已关闭
        /// </summary>
        public bool IsClose
        {
            get { return isClose; }
        }

        /// <summary>
        /// 是否正在发送
        /// </summary>
        public bool IsReceiving
        {
            get
            {
                return isReceiving();
            }
        }

        /// <summary>
        /// 发送成功事件
        /// </summary>
        public Action<ByteBlock> ReceiveCompleted
        {
            get { return receiveCompleted; }
            set { receiveCompleted = value; }
        }

        public ChannelReceiving(Socket socket)
        {
            this.socket = socket;
            this.receiveSocket = new SocketAsyncEventArgs();
            this.receiveSocket.Completed += IOCompleted;
        }

        public void Receive(ByteBlock block)
        { 

            Ensure.IsNotNull(block);

            try
            { 
                receiveSocket.SetBuffer(block.SegmentArray, block.SegmentOffset, block.Length);
                receiveSocket.UserToken = block;

                if (!socket.ReceiveAsync(receiveSocket))
                { 
                    processReceived(receiveSocket);
                } 
            } 
            catch (Exception)
            { 
                shutdownSocket();
            } 

        } 

        private void IOCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                shutdownSocket();
                return;
            }

            else if (e.LastOperation == SocketAsyncOperation.Receive)
                processReceived(e);
        }

        /// <summary>
        /// 关闭socket
        /// </summary>
        private void shutdownSocket()
        {
            isClose = true;
            socket = null;
        }

        private void processReceived(SocketAsyncEventArgs e)
        {

            var block = (ByteBlock)e.UserToken;

            block.SetCount(new ArraySegment<byte>(block.SegmentArray, block.Offset, e.BytesTransferred));

            e.SetBuffer(null, 0, 0);

            receiveCompleted?.Invoke(block);

        }

        /// <summary>
        /// 退出正在接收，关闭正在接收的开关
        /// </summary>
        private void exitReceiving() 
        {
            Interlocked.Exchange(ref receivingMark, 0);
        }

        public void ExitReceiving()
        {
            exitReceiving();
        }

        /// <summary>
        /// 是否正在接收，如果没有正在接收，那么打开正在接收的开关
        /// </summary>
        private bool isReceiving()
        {
            return Interlocked.CompareExchange(ref receivingMark, 1, 0) == 1;
        }
    }

    /// <summary>
    /// 缓冲区，主要负责发送缓冲区的管理
    /// </summary>
    internal class ChannelReceivingBuffer
    {
        private IByteBuffer bytePool;
        private ConcurrentQueue<ByteBlock> receivingQueue;
        private ByteBlock block;

        /// <summary>
        /// 内存池
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="size"></param>
        internal ChannelReceivingBuffer(IByteBuffer pool)
        {
            this.bytePool = pool;
            receivingQueue = new ConcurrentQueue<ByteBlock>();
        }

        public ByteBlock GetBlock() 
        {
            if(block == null)
                block = bytePool.Get();
            return block;
        }

        public void QueueReceving(ByteBlock block)
        {
            receivingQueue.Enqueue(block);
        }

        /// <summary>
        /// 获得接收的
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ByteBlock> getReceivedBlocks()
        {
            //ByteBlock block;

            //while (receivingQueue.TryDequeue(out block))
            //{
            //    yield return block;
            //}

            var blocks = new List<ByteBlock>();

            ByteBlock block;

            while (receivingQueue.TryDequeue(out block))
            {
                blocks.Add(block);
            }

            return blocks;

        }

        public IEnumerable<ByteBlock> GetReceivedBlocks()
        {
            if (isGetReceivedBlocks()) return null;

            IEnumerable<ByteBlock> blocks = null;

            blocks = getReceivedBlocks();

            exitGetReceivedBlocks();

            return blocks;
        }

        public void Return(IEnumerable<ByteBlock> blocks)
        {
            Ensure.IsNotNull(blocks);

            bytePool.Return(blocks);

            block = null;

        }

        private int isGetReceivedBlocksMark = 0;
        private bool isGetReceivedBlocks()
        {
            return Interlocked.CompareExchange(ref isGetReceivedBlocksMark, 1, 0) == 1;
        }

        private void exitGetReceivedBlocks()
        {
            Interlocked.Exchange(ref isGetReceivedBlocksMark, 0);
        }


    }

}
