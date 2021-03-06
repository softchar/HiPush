﻿using Hi.NetWork.Buffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Net.Sockets;
using Hi.Infrastructure.Base;
using System.Threading;
using System.Diagnostics;

namespace Hi.NetWork.Socketing
{
    /// <summary>
    /// 发送流
    /// </summary>
    /// <remarks>
    /// 发送对象
    /// </remarks>
    public class ChannelSendingStream
    { 
        IByteBuffer pool;
        int bufferSize;

        ChannelSendingBuffer sendingBuffer;
        ChannelSending sending;

        public ChannelSendingStream(Socket socket, IByteBuffer pool, int bufferSize)
        { 
            this.pool = pool;
            this.bufferSize = bufferSize;

            this.sendingBuffer = new ChannelSendingBuffer(pool, bufferSize);
            this.sending = new ChannelSending(socket);
            this.sending.SendCompleted = sendCompleted;
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="segment"></param>
        public void Send(IEnumerable<ArraySegment<byte>> segment)
        {
            try
            {
                setSendingQueue(segment);
                trySend();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 将需要发送的数据临时保存在ChannelBuffer中
        /// </summary>
        /// <param name="segment"></param>
        private void setSendingQueue(IEnumerable<ArraySegment<byte>> segment)
        {
            sendingBuffer.SetSendingQueue(segment);
        } 

        /// <summary>
        /// 发送数据
        /// </summary>
        private void trySend()
        {
            if (sending.IsClose) return;
            if (sending.IsSending) return;
             

            if (sendingBuffer.HasValue)
            {
                //从ChannelBuffer中获取需要发送的Block
                var block = sendingBuffer.GetSendingBlock();

                //发送Block
                sending.Send(block);

            }
            else
            {
                sending.ExitSending();
            }

        }

        private void sendCompleted(ByteBlock block)
        {
            sendingBuffer.Return(block);
            sending.ExitSending();
            trySend();
        }

    }

    /// <summary>
    /// 发送,主要负责发送机制
    /// </summary>
    internal class ChannelSending
    {
        private Socket socket;
        private SocketAsyncEventArgs sendingSocket;
        private int sendingMark;
        private bool isClose;
        private Action<ByteBlock> sendCompleted;

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
        public bool IsSending
        {
            get
            {
                return isSending();
            }
        }

        /// <summary>
        /// 发送成功事件
        /// </summary>
        public Action<ByteBlock> SendCompleted
        {
            get { return sendCompleted; }
            set { sendCompleted = value; }
        }

        public ChannelSending(Socket socket)
        {
            this.socket = socket;
            this.sendingSocket = new SocketAsyncEventArgs();
            this.sendingSocket.Completed += IOCompleted;
        }

        public void Send(ByteBlock block)
        {

            Ensure.IsNotNull(block);

            try
            {
                sendingSocket.SetBuffer(block.SegmentArray, block.SegmentOffset, block.Count);
                sendingSocket.UserToken = block;

                if (!socket.SendAsync(sendingSocket))
                {
                    processSend(sendingSocket);
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

            else if (e.LastOperation == SocketAsyncOperation.Send)
                processSend(e);
        }

        /// <summary>
        /// 关闭socket
        /// </summary>
        private void shutdownSocket()
        {
            socket = null;
            sendingSocket = null;
        }

        private void processSend(SocketAsyncEventArgs e)
        {

            var block = (ByteBlock)e.UserToken;
            block.SetCount(new ArraySegment<byte>(block.SegmentArray, block.Offset, e.BytesTransferred));

            if (e.Buffer != null)
            {
                e.SetBuffer(null, 0, 0);
            }
            
            sendCompleted?.Invoke(block);
        }

        /// <summary>
        /// 退出正在发送，关闭正在发送的开关
        /// </summary>
        private void exitSending()
        {
            Interlocked.Exchange(ref sendingMark, 0);
        }

        public void ExitSending()
        {
            exitSending();
        }

        /// <summary>
        /// 是否正在发送，如果没有正在发送，那么打开正在发送的开关
        /// </summary>
        private bool isSending()
        {
            return Interlocked.CompareExchange(ref sendingMark, 1, 0) == 1;
        }
    }

    /// <summary>
    /// 缓冲区，主要负责发送缓冲区的管理
    /// </summary>
    internal class ChannelSendingBuffer
    {
        private IByteBuffer bytePool;
        private int bufferSize = 0;
        private int isCreateSendingBlockMark = 0;
        private ByteBlock block;
        private ConcurrentQueue<ArraySegment<byte>> sendingQueue;
        

        //上一次发送还没有发送完的部分
        private byte[] prevSendingByte;

        /// <summary>
        /// 缓冲区是否还存在数据
        /// </summary>
        internal bool HasValue
        {
            get
            {
                return !sendingQueue.IsEmpty || prevSendingByte != null;
            }
        }

        private object async = new object();

        /// <summary>
        /// 内存池
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="size"></param>
        internal ChannelSendingBuffer(IByteBuffer pool, int bufferSize)
        {
            this.bytePool = pool;
            this.bufferSize = bufferSize;

            sendingQueue = new ConcurrentQueue<ArraySegment<byte>>();
        }

        internal void SetSendingQueue(IEnumerable<ArraySegment<byte>> segments)
        {
            if (segments == null || segments.Count() == 0) return;

            if (segments.Count() > bufferSize)
            {
                segments = slice(segments);
            }

            foreach (var segment in segments)
            {
                sendingQueue.Enqueue(segment);
            }
        }

        /// <summary>
        /// 将segments.count > bufferSize的集合进行分片,确保每一个分片的大小都不大于bufferSize
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        private IEnumerable<ArraySegment<Byte>> slice(IEnumerable<ArraySegment<byte>> segments)
        {
            foreach (var seg in segments)
            {
                int sc = seg.Count();

                if (sc <= bufferSize) yield return seg;

                int mod = sc % bufferSize;
                int n = mod == 0 ? sc / bufferSize : sc / bufferSize + 1;

                for (int i = 0; i < n; i++)
                {
                    yield return new ArraySegment<byte>(seg.Array, i * bufferSize, i == n - 1 ? mod : bufferSize);
                }
            }
        }

        public ByteBlock GetSendingBlock()
        {
            if (isCreateSendingBlock()) return null;

            if (block == null)
            {
                block = bytePool.Get(bufferSize);
            }

            createSendingBlock(block);

            exitCreateSendingBlock();

            return block;
        }

        internal void Return(ByteBlock block)
        {
            bytePool.Return(block);
        }

        private void createSendingBlock(ByteBlock block) 
        { 

            loadPrevSendingByte(ref block); 

            if (block.IsFull()) return; 

            ArraySegment<byte> segment; 

            while (sendingQueue.TryDequeue(out segment)) 
            {

                //如果获得的segment.Count大于block剩余的空间
                if (segment.Count > block.FreeCount)
                {
                    //将block剩余的空间填满
                    var length = block.FreeCount;
                    block.BlockCopy(segment.Array, length);
                    lock (async)
                    {
                        prevSendingByte = new byte[segment.Count - length];
                        System.Buffer.BlockCopy(segment.Array, length, prevSendingByte, 0, segment.Count - length);
                    }
                    
                }
                else
                {
                    block.BlockCopy(segment.Array, segment.Count);
                }

                if (block.IsFull())
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 加载上一次还没有发送完的
        /// </summary>
        /// <param name="block"></param>
        private void loadPrevSendingByte(ref ByteBlock block)
        {
            if (prevSendingByte != null)
            {
                block.BlockCopy(prevSendingByte, prevSendingByte.Length);
                prevSendingByte = null;
            }
        }

        
        private bool isCreateSendingBlock()
        {
            return Interlocked.CompareExchange(ref isCreateSendingBlockMark, 1, 0) == 1;
        }

        private void exitCreateSendingBlock()
        {
            Interlocked.Exchange(ref isCreateSendingBlockMark, 0);
        }
        
    }
}
