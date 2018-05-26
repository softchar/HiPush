using Hi.NetWork.Buffer;
using Hi.NetWork.Socketing.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Hi.NetWork.Socketing.ChannelPipeline
{

    public interface IChannelPipeline : IChannelPipelineExecutor
    {
        IByteBufAllocator Alloc { get; }

        /// <summary>
        /// 设置IChannel
        /// </summary>
        /// <param name="channel"></param>
        void SetChannel(IChannel channel);

        /// <summary>
        /// 设置内存分配器
        /// </summary>
        /// <param name="alloc"></param>
        void SetAlloc(IByteBufAllocator alloc);

        /// <summary>
        /// 将处理程序添加到管道末端
        /// </summary>
        /// <param name="handler"></param>
        void AddLast(string name, IChannelHandler handler);
        
    }
}
