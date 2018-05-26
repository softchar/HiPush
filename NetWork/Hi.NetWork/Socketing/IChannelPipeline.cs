using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing {

    public interface IChannelPipeline {

        /// <summary>
        /// 将处理程序添加到管道末端
        /// </summary>
        /// <param name="handler"></param>
        void AddLast(IChannelHandler handler);

        /// <summary>
        /// 已连接事件
        /// </summary>
        void OnConnected(ChannelPipelineContext cxt);

        /// <summary>
        /// 收到消息事件
        /// </summary>
        /// <param name="message"></param>
        void OnReceived(ChannelPipelineContext ctx,ChannelMessage message);

        /// <summary>
        /// 消息发送事件
        /// </summary>
        void OnSend(ChannelPipelineContext ctx);

        /// <summary>
        /// 断开链接事件
        /// </summary>
        void OnBreaked(ChannelPipelineContext ctx);

        /// <summary>
        /// 重连事件
        /// </summary>
        void OnReconnected(ChannelPipelineContext ctx);

    }
}
