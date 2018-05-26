using Hi.NetWork.Buffer;
using Hi.NetWork.Socketing.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Eventloops
{
    public interface IEventloop
    {
        /// <summary>
        /// 当前线程是否是当前Eventloop指定的线程
        /// </summary>
       bool InEventloop { get; }

        /// <summary>
        /// 内存分配器
        /// </summary>
        IByteBufAllocator Alloc { get; }

        void Execute(IRunnable runable);

        void Execute(ISchedulerRunable task);

        void Register(IChannel channel);

        /// <summary>
        /// 设置ByteBuf
        /// </summary>
        /// <param name="bufAlloc"></param>
        void SetAlloc(IByteBufAllocator bufAlloc);
    }
}
