using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.NetWork.Socketing.Channels;
using Hi.Infrastructure.Base;
using System.Reflection;
using Hi.NetWork.Buffer;
using System.Net;

namespace Hi.NetWork.Socketing.ChannelPipeline
{
    
    /// <summary>
    /// 
    /// </summary>
    public class ChannelHandlerContext : IChannelHandlerContext
    {
        string name;
        IChannel channel;
        IChannelHandler handler;
        IChannelHandlerContext next;
        IChannelHandlerContext prev;
        IByteBufAllocator alloc;
        IChannelPipeline pipeline;
        IByteBuf incompleteMessage;
        int incompleteLength;
        int incompleteHeader;

        protected LifeCycleFlag lifeCycleFlag;

        Dictionary<string, string> implMtddict; 

        public IChannelHandlerContext Prev
        {
            get { return prev; }
            set { prev = value; }
        }

        public IChannelHandlerContext Next
        {
            get { return next; }
            set { next = value; }
        }

        public LifeCycleFlag LifeCycleFlag
        {
            get { return lifeCycleFlag; }
        }

        public IChannel Channel
        {
            get { return channel; }
        }

        public IChannelHandler Handler
        {
            get { return handler; }
        }

        public IByteBufAllocator Alloc
        {
            get {
                return pipeline.Alloc;
            }
        }

        public IByteBuf IncompleteMessage
        {
            get { return incompleteMessage; }
            set { incompleteMessage = value; }
        }

        public int IncompleteLength
        {
            get { return incompleteLength; }
            set { incompleteLength = value; }
        }

        public int IncompleteHeader
        {
            get { return incompleteHeader; }
            set { incompleteHeader = value; }
        }

        public bool IsWritable => channel.OutBoundBuffer.IsWritable;

        public ChannelHandlerContext()
        {
            implMtddict = new Dictionary<string, string>();
        }

        public ChannelHandlerContext(string name, IChannelPipeline pipeline ,IChannel channel, IChannelHandler handler)
            : base()
        {
            Ensure.IsNotOrEmpty(name);
            Ensure.IsNotNull(channel);
            Ensure.IsNotNull(handler);

            this.name = name;
            this.pipeline = pipeline;
            this.channel = channel;
            this.handler = handler;

            CalcSkippable(handler);
        }

        public IChannelHandlerContext SetAlloc(IByteBufAllocator alloc)
        {
            this.alloc = alloc;

            return this;
        }

        /// <summary>
        /// 指定的LifeCycleFlag是否存在于Context的标志位中
        /// </summary>
        /// <param name="cycleFlag"></param>
        /// <returns></returns>
        public bool IsLifeCycle(LifeCycleFlag cycleFlag)
        {
            /************************************************************************/
            /* 假设   lifeCycleFlag = 1111
             *        cycleFlag = 0100
             * 那么   lifeCycleFlag & cycleFlag = 1111 & 0100 = 0100
             * 
             * 从而保证lifeCycleFlag里面有满足的标志位
             *                                                                      */
            /************************************************************************/
            var result = (lifeCycleFlag & cycleFlag) == cycleFlag;

            return result;
        }

        /// <summary>
        /// 计算生命周期标识
        /// </summary>
        /// <param name="handler"></param>
        public void CalcSkippable(IChannelHandler handler)
        {
            Ensure.IsNotNull(handler);

            if (!IsSkippable(handler, nameof(IChannelHandler.OnChannelRegister)))
            {
                lifeCycleFlag |= LifeCycleFlag.OnChannelRegister;
            }
            if (!IsSkippable(handler, nameof(IChannelHandler.OnChannelActive)))
            {
                lifeCycleFlag |= LifeCycleFlag.OnChannelActive; 
            }
            if (!IsSkippable(handler, nameof(IChannelHandler.OnChannelRead)))
            {
                lifeCycleFlag |= LifeCycleFlag.OnChannelRead;
            }
            if (!IsSkippable(handler, nameof(IChannelHandler.OnChannelWrite)))
            {
                lifeCycleFlag |= LifeCycleFlag.OnChannelWrite;
            }
            if (!IsSkippable(handler, nameof(IChannelHandler.OnChannelClose)))
            {
                lifeCycleFlag |= LifeCycleFlag.OnChannelClose;
            }
            if (!IsSkippable(handler, nameof(IChannelHandler.OnChannelException)))
            {
                lifeCycleFlag |= LifeCycleFlag.OnChannelException;
            }
            if (!IsSkippable(handler, nameof(IChannelHandler.OnChannelFinally)))
            {
                lifeCycleFlag |= LifeCycleFlag.OnChannelFinally;
            }
            if (!IsSkippable(handler, nameof(IChannelHandler.WriteAsync)))
            {
                lifeCycleFlag |= LifeCycleFlag.WriteAsync;
            }
            if (!IsSkippable(handler, nameof(IChannelHandler.BindAsync)))
            {
                lifeCycleFlag |= LifeCycleFlag.BindAsync;
            }
            if (!IsSkippable(handler, nameof(IChannelHandler.ConnectAsync)))
            {
                lifeCycleFlag |= LifeCycleFlag.ConnectAsync;
            }

        }

        /// <summary>
        /// 指定函数名称的函数是否被标记为Skip（不包含父类）
        /// </summary>
        /// <param name="handlerType"></param>
        /// <param name="methodName"></param>
        /// <param name="paramTypes"></param>
        /// <returns></returns>
        protected virtual bool IsSkippable(IChannelHandler handler, string methodName) 
        {
            var handlerType = handler.GetType();

            var attr = handlerType.GetMethod(methodName).GetCustomAttribute<SkipAttribute>(false);

            return attr != null;
        }

        public IChannelHandlerContext FindInboundContext(LifeCycleFlag lifeCycleFlag)
        {
            IChannelHandlerContext ctx = this;
            do 
            {
                ctx = ctx.Next;

            } while (ctx != null && !ctx.IsLifeCycle(lifeCycleFlag));

            return ctx;

        }

        public IChannelHandlerContext FindOutboundContext(LifeCycleFlag lifeCycleFlag)
        {
            IChannelHandlerContext ctx = this;
            do 
            {
                ctx = ctx.Prev;

            } while (ctx != null && !ctx.IsLifeCycle(lifeCycleFlag));

            return ctx;
        }

        public void fireChannelRegister()
        {
            var ctx = FindInboundContext(LifeCycleFlag.OnChannelRegister);
            ctx?.Handler.OnChannelRegister(ctx);
        }

        public void fireChannelActive()
        {
            var ctx = FindInboundContext(LifeCycleFlag.OnChannelActive);
            ctx?.Handler.OnChannelActive(ctx);
        }

        public void fireChannelRead(object message)
        {
            var ctx = FindInboundContext(LifeCycleFlag.OnChannelRead);
            ctx?.Handler.OnChannelRead(ctx, message);
        }

        public void fireChannelWrite(object message)
        {
            var ctx = FindInboundContext(LifeCycleFlag.OnChannelWrite);
            ctx?.Handler.OnChannelWrite(ctx, message);
            
        }

        public Task WriteAsync(object message)
        {
            var ctx = FindOutboundContext(LifeCycleFlag.WriteAsync);
            return ctx?.Handler.WriteAsync(ctx, message);
        }

        public void fireChannelClose()
        {
            throw new NotImplementedException();
        }

        public void fireChannelException()
        {
            throw new NotImplementedException();
        }

        public void fireChannelFinally()
        {
            throw new NotImplementedException();
        }

        public Task BindAsync(EndPoint remote)
        {
            var ctx = FindOutboundContext(LifeCycleFlag.BindAsync);
            return ctx?.Handler.BindAsync(ctx, remote);
        }

        public Task ConnectAsync(EndPoint remote)
        {
            var ctx = FindOutboundContext(LifeCycleFlag.ConnectAsync);
            return ctx?.Handler.ConnectAsync(ctx, remote);
        }

        private object FindOutboundContext(object connectAsync)
        {
            throw new NotImplementedException();
        }
    }
}
