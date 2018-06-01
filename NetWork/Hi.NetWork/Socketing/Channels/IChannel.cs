using Hi.NetWork.Buffer;
using Hi.NetWork.Eventloops;
using Hi.NetWork.Socketing.ChannelPipeline;
using Hi.NetWork.Socketing.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing.Channels {

    /************************************************************************/
    /* 为Channel提供对外服务,作用类似于HttpContext,                           */
    /************************************************************************/
    public interface IChannel {

        IChannelInvoker Invoker { get; }

        /// <summary>
        /// Channel事件管道
        /// </summary>
        IChannelPipeline Pipeline { get; }

        /// <summary>
        /// 出站缓冲区
        /// </summary>
        OutBoundBuffer OutBoundBuffer { get; }

        /// <summary>
        /// 通道状态
        /// </summary>
        ChannelStatus ChannelStatus { get; }

        /// <summary>
        /// 是否是活动状态
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// 事件循环器
        /// </summary>
        IEventloop Eventloop { get;}

        /// <summary>
        /// 已连接事件
        /// </summary>
        /// <param name="socket"></param>
        //IChannel OnAccepted();

        IChannel SetPipeline(Action<IChannelPipeline> setPipeline);

        IChannel SetConfig(ChannelConfig config);

        /// <summary>
        /// 获得远程的节点信息
        /// </summary>
        /// <returns></returns>
        EndPoint GetRemoteNode();

        /// <summary>
        /// 异步绑定(该方法会经过Pipeline)
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Task BindAsync(EndPoint address = null);

        /// <summary>
        /// 异步激活
        /// </summary>
        /// <returns></returns>
        Task ActiveAsync();

        /// <summary>
        /// 异步连接(该方法会经过pipeline)
        /// 
        /// 客户端方法
        /// </summary>
        /// <param name="remote"></param>
        /// <returns></returns>
        Task ConnectAsync(EndPoint remote = null);

        /// <summary>
        /// 异步写
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        Task WriteAsync(IByteBuf buf);

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Task DoBind(EndPoint address = null);

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="remote"></param>
        /// <returns></returns>
        Task DoConnect(EndPoint remote = null);

    }
}
