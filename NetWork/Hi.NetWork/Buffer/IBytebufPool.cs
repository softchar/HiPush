using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Buffer
{
    public interface IBytebufPool
    {
        
        /// <summary>
        /// bytebuf最大值
        /// </summary>
        int MaxBufferCounter { get; }

        /// <summary>
        /// 可用数
        /// </summary>
        int Available { get; }

        /// <summary>
        /// bytebuf的数量（Available + References）
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 引用计数
        /// </summary>
        int References { get; }

        /// <summary>
        /// 创建ByteBuf处理函数
        /// </summary>
        /// <remarks>
        /// 当T对象需要更复杂的构造时，create handle能提供更好的支持
        /// </remarks>
        Func<IByteBuf> NewByteBufHandle { get; set; }

        /// <summary>
        /// 是否可用（Available>0）
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// 获取一个Bytebuf
        /// </summary>
        /// <returns></returns>
        IByteBuf Get();

        /// <summary>
        /// 释放一个Bytebuf
        /// </summary>
        /// <returns>True：释放成功； False：释放失败</returns>
        bool Return(IByteBuf buf);
    }
}
