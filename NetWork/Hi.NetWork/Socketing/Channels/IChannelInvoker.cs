using Hi.NetWork.Buffer;
using Hi.NetWork.Eventloops;
using Hi.NetWork.Socketing.ChannelPipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing.Channels
{
    /// <summary>
    /// 执行器，执行channel-pipeline, channel-eventloop操作
    /// </summary>
    public interface IChannelInvoker
    {
        IEventloop Eventloop { get; }

        IChannelPipeline Pipeline { get; }

        IByteBufAllocator Alloc { get; }

        /// <summary>
        /// 当前线程是否是Eventloop线程
        /// </summary>
        /// <returns></returns>
        bool InEventloop { get; }

        /// <summary>
        /// 注册Eventloop
        /// </summary>
        /// <param name="loop"></param>
        Task Register(IEventloop loop);

        /// <summary>
        /// 执行通道已注册到Pipeline事件
        /// </summary>
        Task fireOnChannelRegister(TaskCompletionSource promise);

        /// <summary>
        /// 执行通道激活，可以进行读取和写入操作
        /// </summary>
        Task fireOnChannelActive(TaskCompletionSource promise, Action callback = null);

        /// <summary>
        /// 执行通道重连事件
        /// </summary>
        Task fireOnChannelDisConnection(TaskCompletionSource promise);

        /// <summary>
        /// 执行通道读取事件
        /// </summary>
        /// <param name="msg"></param>
        void fireOnChannelRead(object msg, Action callback = null);

        /// <summary>
        /// 执行发送事件
        /// </summary>
        void fireOnChannelSend(object buf, Action callback = null);

        /// <summary>
        /// 执行通道关闭事件（手动关闭），此时通道的资源还不会被释放，可以重新启用
        /// </summary>
        Task fireOnChannelClose(TaskCompletionSource promise);

        /// <summary>
        /// 执行通道终止事件
        /// </summary>
        Task fireOnShutdown(TaskCompletionSource promise);

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Task fireBindAsync(EndPoint address);

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="remote"></param>
        /// <returns></returns>
        Task fireConnectAsync(EndPoint remote);

        void Execute(Action action);

        /// <summary>
        /// 下次执行
        /// </summary>
        /// <param name="action"></param>
        void NextTimeExecute(Action action);
        
    }
}
