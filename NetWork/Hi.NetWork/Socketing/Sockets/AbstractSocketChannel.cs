using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hi.Infrastructure.Base;
using Hi.NetWork.Socketing.Channels;
using Hi.NetWork.Buffer;
using Hi.NetWork.Protocols;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.IO;
using Hi.NetWork.Eventloops;
using System.Net;
using System.Runtime.CompilerServices;

namespace Hi.NetWork.Socketing.Sockets
{

    /************************************************************************/
    /* Channel中的Socket操作
     * 1：发送
     * 2：接受
     * 3：关闭连接
     * 4：重新连接                                                                   */
    /************************************************************************/

    /// <summary>
    /// 通道内Socket有关的操作
    /// </summary>
    public abstract class AbstractSocketChannel : AbstractChannel, IChannel
    {
        protected readonly ConcurrentQueue<IByteBuf> sendingQueue1 = new ConcurrentQueue<IByteBuf>();
        protected readonly ConcurrentQueue<WaitingMsg> sendingQueue = new ConcurrentQueue<WaitingMsg>();
        protected readonly MemoryStream sendingStream = new MemoryStream();
        /************************************************************************/

        private int counterSendingBytes = 0;        //已发送的字节
        private int sendingMark;                    //发送标志
        private int receivingMark;                  //接收标志

        private int incompleteWriteMsgCounter = 0;  //等待写的消息的计数

        private Socket socket;
        private ChannelConfig config;               //配置

        private ChannelSocketAsyncEventArgs sendingSocketAsyncEventArgs;
        private ChannelSocketAsyncEventArgs receivingSocketAsyncEventArgs;
        private ManualResetEventSlim sendEvent = new ManualResetEventSlim(false);

        /// <summary>
        /// 发送缓冲区的大小
        /// </summary>
        public int SendingBufferSize
        {
            get { return config.SendingBufferSize; }
        }

        /// <summary>
        /// 接收缓冲区的大小
        /// </summary>
        public int ReceivingBufferSize
        {
            get { return config.ReceivingBufferSize; }
        }

        public Socket Socket
        {
            get
            {
                return socket;
            }
        }

        public AbstractSocketChannel()
        {
            this.sendingSocketAsyncEventArgs = new ChannelSocketAsyncEventArgs();
            this.receivingSocketAsyncEventArgs = new ChannelSocketAsyncEventArgs();

            this.sendingSocketAsyncEventArgs.Completed += IO_Completed;
            this.receivingSocketAsyncEventArgs.Completed += IO_Completed;

            config = new ChannelConfig();
        }

        public AbstractSocketChannel(Socket socket)
            : this()
        {
            this.socket = socket;
        }

        /// <summary>
        /// 异步激活
        /// </summary>
        /// <returns></returns>
        public override Task ActiveAsync()
        {
            var promise = new TaskCompletionSource();

            Execute(() => 
            {
                receivingByteBuf = Alloc.Buffer(this.ReceivingBufferSize);
                receivingSocketAsyncEventArgs.ByteBuf = receivingByteBuf;

                SetConnection();

                //设置读取缓冲区,将状态设置为Activity之后就可以直接完成任务了

                invoker.fireOnChannelActive(promise);

                TryReceiving();
            });

            return promise.Task;

        } 

        public abstract override Task BindAsync(EndPoint address = null);

        public abstract override Task ConnectAsync(EndPoint remote = null);

        /// <summary>
        /// 异步写
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        public override Task WriteAsync(IByteBuf buf)
        {
            Ensure.IsNotNull(buf != null);

            var promise = new TaskCompletionSource();

            while (incompleteWriteMsgCounter >= config.PenddingMessageCounter)
            {
                //Thread.SpinWait(3000);
                Thread.Sleep(1);
                TrySending();
            }

            Interlocked.Increment(ref incompleteWriteMsgCounter);

            //建议不要`new WaitingMsg()`而是使用对象池
            sendingQueue.Enqueue(new WaitingMsg(promise, buf));
            //sendingQueue1.Enqueue(buf);

            TrySending();

            return promise.Task;
        }

        public abstract override Task DoBind(EndPoint address = null);

        public abstract override Task DoConnect(EndPoint remote = null);

        /// <summary>
        /// 设置配置信息
        /// </summary>
        /// <param name="config"></param>
        public override IChannel SetConfig(ChannelConfig config)
        {
            if (config != null)
            {
                this.config = config;
            }

            return this;
        }

        public override EndPoint GetRemoteNode()
        {
            return socket.RemoteEndPoint;
        }

        /// <summary>
        /// 尝试去发送数据
        /// </summary>
        protected void TrySending()
        {
            //确保eventloop的任务列表中最多只有一个发送的操作
            if (this.IsSending())
                return;

            Execute(TrySending0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TrySending0()
        {
            if (!IsAlive) return;

            sendingStream.SetLength(0);

            WaitingMsg msg;

            while (sendingQueue.TryDequeue(out msg))
            {
                Interlocked.Decrement(ref incompleteWriteMsgCounter);

                sendingStream.Write(msg.buf.GetBytes(), msg.buf.ReadIndex, msg.buf.Readables());

                msg.promise.Success();

                msg.buf.Return();

                if (sendingStream.Length >= config.SendingBufferSize)
                    break;
            }

            if (sendingStream.Length > 0)
            {
                Sending();
            }
            else
            {
                ExitSending();
            }
        }

        private void TrySending1()
        {
            if (!IsAlive) return;
            if (this.IsSending()) return;

            sendingStream.SetLength(0);

            IByteBuf buf;

            while (sendingQueue1.TryDequeue(out buf))
            {
                Interlocked.Decrement(ref incompleteWriteMsgCounter);

                sendingStream.Write(buf.GetBytes(), buf.ReadIndex, buf.Readables());

                buf.Return();

                if (sendingStream.Length >= config.SendingBufferSize)
                    break;
            }

            if (sendingStream.Length > 0)
            {
                Sending();
            }
            else
            {
                ExitSending();
            }
        }

        /// <summary>
        /// 尝试去接收数据
        /// </summary>
        protected void TryReceiving()
        {
            if (!IsAlive || IsReceiving()) return;

            Execute(TryReceiving0);
        }

        private void TryReceiving0()
        {
            if (!IsAlive) return;

            Execute(Receiving);
        }

        /// <summary>
        /// 关闭通道
        /// </summary>
        public override Task Shutdown()
        {
            var promise = new TaskCompletionSource();
            try
            {
                Ensure.IsNotNull(socket);

                Execute(() =>
                {
                    SetShutdown();

                    ExitSending();
                    ExitReceiving();

                    //释放接收缓冲区
                    this.receivingByteBuf.Return();
                    this.receivingByteBuf = null;
                    this.sendingStream.Dispose();

                    socket.Close();
                    socket = null;

                    invoker.fireOnShutdown(promise);
                });
            }
            catch (System.Exception ex)
            {
                
            }

            return promise.Task;

        }

        protected void IO_Completed(object sender, SocketAsyncEventArgs args)
        {
            //操作成功
            if (args.SocketError == SocketError.Success)
            {
                ChannelSocketAsyncEventArgs eargs = args as ChannelSocketAsyncEventArgs;
                ProcessCompleted(eargs);
                return;
            }
            //收到断开连接的请求
            else if (args.SocketError == SocketError.ConnectionReset)
            {
                Trace.WriteLine($"args.SocketError: SocketError.ConnectionReset");
                Shutdown();
                return;
            }
            else if (args.SocketError == SocketError.WouldBlock)
            {
                Trace.WriteLine($"args.SocketError: SocketError.WouldBlock");
            }
            else
            {
                Trace.WriteLine($"args.SocketError:{ args.SocketError }");
                Shutdown();
                return;
            }

        }

        protected virtual void ProcessCompleted(ChannelSocketAsyncEventArgs args)
        {
            switch (args.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(args);
                    break;

                case SocketAsyncOperation.Send:
                    ProcessSend(args);
                    break;

                case SocketAsyncOperation.Disconnect:
                    Trace.WriteLine("Disconnect");
                    break;
                default:
                    Trace.WriteLine("default");
                    break;
            }
        }

        protected void Sending()
        {
            try
            {
                sendingSocketAsyncEventArgs.SetBuffer(sendingStream.GetBuffer(), 0, (int)sendingStream.Length);

                if (!socket.SendAsync(sendingSocketAsyncEventArgs))
                {
                    ProcessSend(sendingSocketAsyncEventArgs);
                }

            }
            catch (Exception sktExcep)
            {
                Trace.WriteLine("sending:{0}", sktExcep.Message);
                Shutdown();
            }
        }

        /// <summary>
        /// 让socket接收数据
        /// </summary>
        protected void Receiving()
        {
            try
            {
                receivingByteBuf.Clear();
                receivingSocketAsyncEventArgs.SetBuffer(receivingByteBuf.GetBytes(), receivingByteBuf.WriteIndex, ReceivingBufferSize);
                receivingSocketAsyncEventArgs.ByteBuf = receivingByteBuf;

                if (!socket.ReceiveAsync(receivingSocketAsyncEventArgs))
                {
                    ProcessReceive(receivingSocketAsyncEventArgs);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("receiving:" + ex.Message);
                Shutdown();
            }

        }

        private void ProcessSend(ChannelSocketAsyncEventArgs args)
        {
            if (args.Buffer != null)
            {
                args.SetBuffer(null, 0, 0);
            }

            ExitSending();
            TrySending();

            //因为这里发送没有使用Bytebuf,即ByteBuf=null,否则应该放在ExitSending的前面
            invoker.fireOnChannelSend(args.ByteBuf);

        }

        /// <summary>
        /// 接收完成并处理数据
        /// </summary>
        /// <param name="args"></param>
        private void ProcessReceive(ChannelSocketAsyncEventArgs args)
        {
            try
            {
                if (args.BytesTransferred > 0)
                {
                    IByteBuf buf = args.ByteBuf;
                    buf.SetWriteIndex(args.BytesTransferred);
                    invoker.fireOnChannelRead(buf);

                    Execute(() =>
                    {
                        ExitReceiving();
                        TryReceiving();
                    });
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("processReceive.Unpacking:" + ex.Message);
                Shutdown();
            }
        }

        /// <summary>
        /// 是否正在接收数据,如果没有在接受数据那么打开正在接收的开关
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool IsReceiving()
        {
            return Interlocked.CompareExchange(ref receivingMark, 1, 0) == 1;
        }

        /// <summary>
        /// 退出正在接受数据
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ExitReceiving()
        {
            Interlocked.Exchange(ref receivingMark, 0);
        }

        /// <summary>
        /// 是否正在发送，如果没有正在发送数据那么打开正在发送的开关
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool IsSending()
        {
            return Interlocked.CompareExchange(ref sendingMark, 1, 0) == 1;
        }

        /// <summary>
        /// 退出正在发送，关闭正在发送的开关
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ExitSending()
        {
            Interlocked.Exchange(ref sendingMark, 0);
        }

        /// <summary>
        /// 等待发送的消息
        /// </summary>
        protected class WaitingMsg
        {
            public TaskCompletionSource promise;
            public IByteBuf buf;

            public WaitingMsg(TaskCompletionSource promise, IByteBuf buf)
            {
                this.promise = promise;
                this.buf = buf;
            }

        }

    }
}
