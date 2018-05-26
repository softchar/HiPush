using Hi.NetWork.Buffer;
using Hi.NetWork.Socketing.Channels;
using System.Net;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing.ChannelPipeline
{
    public interface IChannelHandlerContext : IChannelPipelineExecutor
    {
        /// <summary>
        /// 半包消息
        /// </summary>
        IByteBuf IncompleteMessage { get; set; }

        /// <summary>
        /// 读取包的长度时，读取的位数，范围在0-4
        /// </summary>
        int IncompleteLength { get; set; }

        /// <summary>
        /// 消息的长度
        /// </summary>
        int IncompleteHeader { get; set; }

        IByteBufAllocator Alloc { get; }
        IChannelHandlerContext Prev { get; set; }

        IChannelHandlerContext Next { get; set; }

        IChannel Channel { get; }
        IChannelHandler Handler { get; }

        /// <summary>
        /// 生命周期事件标志位
        /// </summary>
        LifeCycleFlag LifeCycleFlag { get; }
        

        /// <summary>
        /// 判断指定的LifeCycleFlag是否存在于当前Context的LifeCycleFlag标志位中
        /// </summary>
        /// <param name="lifeCycleFlag"></param>
        /// <returns></returns>
        bool IsLifeCycle(LifeCycleFlag lifeCycleFlag);

    }
}