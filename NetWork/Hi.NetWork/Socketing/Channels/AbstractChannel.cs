using Hi.Infrastructure.Base;
using Hi.NetWork.Buffer;
using Hi.NetWork.Protocols;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hi.NetWork.Eventloops;
using Hi.NetWork.Socketing.ChannelPipeline;
using System.Diagnostics;
using System.Net;
using Hi.NetWork.Socketing.Sockets;

namespace Hi.NetWork.Socketing.Channels
{

    /************************************************************************/
    /* 通道生命周期
     * 1，Active，执行pipeline的发送fireActive事件
     * 2，Receive，执行pipeline的fireRead事件，这个事件主要做数据的解码的工作
     * 3，ReceiveCompleted，执行pipeline的fireReadCompleted，此时说明解码已完成
     * 4，Send，执行pipeline的fireSend事件，这个事件主要做数据的编码工作
     * 4，SendCompleted，执行pepeline的fireSendCompleted，此时说明发送已完成
     * 5，Closed，执行pipeline的fireClosed，关闭Channel的发送和接受操作
     * 6，Disconnected，执行pipeline的fireDisconnected，表示Channel已经断开连接
    /************************************************************************/

    /// <summary>
    /// 抽象的Channel通信通道,主要负责Channel与Pipeline、Eventloop的操作
    /// </summary>
    public abstract class AbstractChannel : IChannel
    {
        private ChannelStatus channelStatus;        //通道状态
        private bool isAlive;                       //是否可用

        protected IChannelInvoker invoker;           //事件执行器
        protected IByteBuf receivingByteBuf;        //接收字节缓冲区
        protected OutBoundBuffer outBoundBuffer;

        /// <summary>
        /// 事件循环
        /// </summary>
        public IEventloop Eventloop
        {
            get
            {
                return invoker.Eventloop;
            }
        }

        /// <summary>
        /// 事件处理通道
        /// </summary>
        public IChannelPipeline Pipeline
        {
            get
            {
                return invoker.Pipeline;
            }
        }

        /// <summary>
        /// 通道状态
        /// </summary>
        public ChannelStatus ChannelStatus
        {
            get
            {
                return channelStatus;
            }
        }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return isAlive;
            }
        }

        public IChannelInvoker Invoker
        {
            get { return invoker; }
        }

        public IByteBufAllocator Alloc
        {
            get { return invoker.Alloc; }
        }

        public OutBoundBuffer OutBoundBuffer => outBoundBuffer;

        public AbstractChannel()
        {
            this.outBoundBuffer = new OutBoundBuffer();
            this.invoker = NewChannelInvoker();
        }

        protected virtual IChannelInvoker NewChannelInvoker() => new ChannelInvoker(this);

        public IChannel SetPipeline(Action<IChannelPipeline> setPipeline)
        {
            setPipeline(this.Pipeline);

            return this;
        }

        /// <summary>
        /// Channel已连接
        /// </summary>
        /// <param name="socket"></param>
        public virtual Task ActiveAsync()
        {
            var promise = new TaskCompletionSource();
            SetConnection();
            return invoker.fireOnChannelActive(promise);
        }

        /// <summary>
        /// 获得远程节点的信息
        /// </summary>
        /// <returns></returns>
        public abstract EndPoint GetRemoteNode();

        /// <summary>
        /// 通道关闭
        /// </summary>
        protected void SetClose()
        {
            this.channelStatus = ChannelStatus.Close;
            //关闭socket，并允许重用
            this.isAlive = false;
        }

        /// <summary>
        /// 通道连接
        /// </summary>
        protected void SetConnection()
        {
            this.channelStatus = ChannelStatus.Connection;
            this.isAlive = true;
        }

        /// <summary>
        /// 通道终止
        /// </summary>
        protected void SetShutdown()
        {
            this.channelStatus = ChannelStatus.Shutdown;
            this.isAlive = false;
        }

        protected void Execute(Action action)
        {
            Ensure.IsNotNull(action);

            invoker.Execute(action);
        }

        public virtual Task Shutdown()
        {
            var promise = new TaskCompletionSource();
            Execute(() => 
            {
                this.channelStatus = ChannelStatus.Shutdown;
                this.receivingByteBuf.Return();
            });
            return invoker.fireOnShutdown(promise);
        }

        public abstract Task BindAsync(EndPoint address = null);

        public abstract Task ConnectAsync(EndPoint remote = null);

        public abstract Task WriteAsync(IByteBuf buf);

        public abstract Task DoBind(EndPoint address = null);

        public abstract Task DoConnect(EndPoint remote = null);

        public abstract IChannel SetConfig(ChannelConfig config);

        /// <summary>
        /// 通道执行器
        /// </summary>
        /// <remarks>
        /// 执行Channel到Pipeline过程的执行器
        /// </remarks>
        internal class ChannelInvoker : IChannelInvoker
        {
            AbstractChannel parent;
            IChannelPipeline pipeline;
            IEventloop eventloop;
            IByteBufAllocator alloc;

            public IChannelPipeline Pipeline { get { return pipeline; } }
            public IEventloop Eventloop { get { return eventloop; } }
            public IByteBufAllocator Alloc { get { return alloc; } }

            public ChannelInvoker(AbstractChannel parent)
            {
                this.parent = parent;
                this.pipeline = new DefaultChannelPipeline(parent);
            }

            /// <summary>
            /// 当前线程是否是Eventloop线程
            /// </summary>
            /// <returns></returns>
            public bool InEventloop => eventloop == null ? false : eventloop.InEventloop;

            /// <summary>
            /// 执行通道已注册到Eventloop事件
            /// </summary>
            public Task fireOnChannelRegister(TaskCompletionSource promise)
            {
                Execute(() =>
                {
                    alloc = NewByteBufAlloc();
                    pipeline.SetAlloc(alloc);
                    if (eventloop.Alloc == null)
                    {
                        eventloop.SetAlloc(alloc);
                    }
                    pipeline.fireChannelRegister();
                });
                return promise.Task;
            }

            /// <summary>
            /// 执行通道连接事件，可以进行读取和写入操作
            /// </summary>
            public Task fireOnChannelActive(TaskCompletionSource promise, Action callback = null)
            {
                Execute(() =>
                {
                    pipeline.fireChannelActive();
                    callback?.Invoke();
                    promise.Success();
                });
                return promise.Task;
            }

            /// <summary>
            /// 执行通道读取事件
            /// </summary>
            /// <param name="msg"></param>
            public void fireOnChannelRead(object msg, Action callback = null)
            {
                Execute(() =>
                {
                    pipeline.fireChannelRead(msg);
                    callback?.Invoke();
                });
            } 

            /// <summary>
            /// 执行发送事件
            /// </summary>
            public void fireOnChannelSend(object buf, Action callback = null)
            {
                Execute(() =>
                {
                    pipeline.fireChannelWrite(buf);
                    callback?.Invoke();
                });
            }

            /// <summary>
            /// 执行通道关闭事件（手动关闭），此时通道的资源还不会被释放，可以重新启用
            /// </summary>
            public Task fireOnChannelClose(TaskCompletionSource promise)
            {
                parent.SetClose();
                Execute(() =>
                {
                    pipeline.fireChannelClose();
                    promise.Success();
                });
                return promise.Task;
            }

            /// <summary>
            /// 执行通道终止事件
            /// </summary>
            public Task fireOnShutdown(TaskCompletionSource promise)
            {
                parent.SetShutdown();
                promise.Success();
                return promise.Task;
            }

            public void Execute(Action action)
            {
                Ensure.IsNotNull(action);

                if (eventloop.InEventloop)
                {
                    action();
                }
                else
                {
                    eventloop.Execute(new ActionTask(action));
                }
            }

            public void NextTimeExecute(Action action)
            {
                Ensure.IsNotNull(action);

                eventloop.Execute(new ActionTask(action));
            }

            public Task Register(IEventloop loop)
            {
                var promise = new TaskCompletionSource();
                eventloop = loop;
                return fireOnChannelRegister(promise);
            }

            protected virtual IByteBufAllocator NewByteBufAlloc()
            {
                return DefaultByteBufAllocator.Default;
            }

            public Task fireBindAsync(EndPoint remote)
            {
                 return pipeline.BindAsync(remote);
            }

            public Task fireConnectAsync(EndPoint remote)
            {
                return pipeline.ConnectAsync(remote);
            }

            public Task fireOnChannelDisConnection(TaskCompletionSource promise)
            {
                promise.Success();
                return promise.Task;
            }

            
        }
    }

    
}
