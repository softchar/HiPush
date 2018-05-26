using Hi.Infrastructure.EventHandle;
using Hi.NetWork.Buffer;
using Hi.NetWork.Pipeline;
using Hi.NetWork.Socketing.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing.ChannelPipeline
{

    public interface IChannelHandler {


        /// <summary>
        /// Channel注册事件
        /// </summary>
        /// <param name="context"></param>
        void OnChannelRegister(IChannelHandlerContext context);

        /// <summary>
        /// Channel激活事件
        /// </summary>
        /// <param name="context"></param>
        void OnChannelActive(IChannelHandlerContext context);

        /// <summary>
        /// Channel读取
        /// </summary>
        /// <param name="context"></param>
        /// <param name="buf"></param>
        void OnChannelRead(IChannelHandlerContext context, object message);

        /// <summary>
        /// Channel写入
        /// </summary>
        /// <param name="context"></param>
        /// <param name="buf"></param>
        void OnChannelWrite(IChannelHandlerContext context, object message);

        /// <summary>
        /// Channel关闭
        /// </summary>
        /// <param name="context"></param>
        void OnChannelClose(IChannelHandlerContext context);

        /// <summary>
        /// Channel异常
        /// </summary>
        /// <param name="context"></param>
        void OnChannelException(IChannelHandlerContext context);

        /// <summary>
        /// Channel终结
        /// </summary>
        /// <param name="context"></param>
        void OnChannelFinally(IChannelHandlerContext context);

        /// <summary>
        /// 异步写
        /// </summary>
        /// <param name="channelHandlerContext"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task WriteAsync(IChannelHandlerContext channelHandlerContext, object message);

        /// <summary>
        /// 异步绑定
        /// </summary>
        /// <param name="channelHandlerContext"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        Task BindAsync(IChannelHandlerContext channelHandlerContext, EndPoint address);

        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="remote"></param>
        /// <returns></returns>
        Task ConnectAsync(IChannelHandlerContext channelHandlerContext, EndPoint remote);
    }
}
