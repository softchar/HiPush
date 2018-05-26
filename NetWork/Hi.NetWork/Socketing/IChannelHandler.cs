using Hi.Infrastructure.EventHandle;
using Hi.NetWork.Pipeline;
using Hi.NetWork.Socketing.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing {

    public interface IChannelHandler {

        ///// <summary>
        ///// 设置下一个处理对象
        ///// </summary>
        ///// <param name="handler"></param>
        void SetNext(IChannelHandler handler);

        ///// <summary>
        ///// 获得下一个处理对象
        ///// </summary>
        ///// <returns></returns>
        IChannelHandler Next();

        /// <summary>
        /// 已连接
        /// </summary>
        /// <param name="channel">通道上下文</param>
        /// <returns>是否继续往下执行</returns>
        bool OnConnected(IChannel channel);

        /// <summary>
        /// 已接受
        /// </summary>
        /// <returns>是否继续往下执行</returns>
        bool OnReceived(IChannel channel, ChannelMessage message);

        /// <summary>
        /// 已发送
        /// </summary>
        /// <returns>是否继续往下执行</returns>
        bool OnSend(IChannel channel);

        /// <summary>
        /// 已断开连接
        /// </summary>
        /// <returns>是否继续往下执行</returns>
        bool OnBreaked();

        /// <summary>
        /// 已重连
        /// </summary>
        /// <returns>是否继续往下执行,默认为True</returns>
        bool OnReconnected();
       
    }
}
