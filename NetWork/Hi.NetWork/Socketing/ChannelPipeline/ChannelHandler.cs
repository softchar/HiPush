using Hi.NetWork.Socketing.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.NetWork.Buffer;
using System.Net;

namespace Hi.NetWork.Socketing.ChannelPipeline
{
    public abstract class ChannelHandler : IChannelHandler
    {
        [Skip]
        public virtual void OnChannelRegister(IChannelHandlerContext context) { }

        [Skip]
        public virtual void OnChannelActive(IChannelHandlerContext context) { }

        [Skip]
        public virtual void OnChannelRead(IChannelHandlerContext context, object message) { }

        [Skip]
        public virtual void OnChannelWrite(IChannelHandlerContext context, object message) { }

        [Skip]
        public virtual void OnChannelClose(IChannelHandlerContext context) { }

        [Skip]
        public virtual void OnChannelException(IChannelHandlerContext context) { }

        [Skip]
        public virtual void OnChannelFinally(IChannelHandlerContext context) { }

        [Skip]
        public virtual Task WriteAsync(IChannelHandlerContext context, object message) => context.Channel.WriteAsync((IByteBuf)message);

        [Skip]
        public virtual Task BindAsync(IChannelHandlerContext context, EndPoint remote) => context.Channel.BindAsync(remote);

        [Skip]
        public virtual Task ConnectAsync(IChannelHandlerContext context, EndPoint remote) => context.Channel.DoConnect(remote);
    }

}
