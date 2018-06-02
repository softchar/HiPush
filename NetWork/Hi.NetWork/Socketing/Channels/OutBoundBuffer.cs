using Hi.Infrastructure.Base;
using Hi.NetWork.Buffer;
using Hi.NetWork.Eventloops;
using Hi.NetWork.Socketing.Channels.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing.Channels
{
    /*
     * 出站消息缓冲区
     */ 

    /// <summary>
    /// 
    /// </summary>
    public class OutBoundBuffer : /*BlockingCollection<PendingMessage>*/ConcurrentQueue<PendingMessage>
    {
        private static int DefaultMaxCount = 16384;
       
        /// <summary>
        /// 计数器的最大值
        /// </summary>
        private int maxCount;

        /// <summary>
        /// 计数器的最大值
        /// </summary>
        public int MaxCount => maxCount;

        public bool IsWritable => Count < maxCount;

        public OutBoundBuffer()
            : this(DefaultMaxCount)
        {
            
        }

        public OutBoundBuffer(int maxCount)
        {
            this.maxCount = maxCount;
        }

        public void Add(PendingMessage msg)
        {
            //EnsureMaxCount();

            Enqueue(msg);
        }

        public PendingMessage Get()
        {
            PendingMessage msg = null;
            if (this.TryPeek(out msg))
            {
                TryDequeue(out msg);
            }
            return msg;
        }

        /// <summary>
        /// 确保添加的数量不会超过MaxCount
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureMaxCount()
        {
            if (Count==maxCount)
                throw new OutOfBufferException();
        }

    } 

    /*
     * 等待的消息，实际是一个链表，这里默认使用双向链表，其实更好的方法是使用双向链表
     */ 
    public class PendingMessage : HiLinkNode<PendingMessage>
    {
        internal IByteBuf Buf;
        internal TaskCompletionSource Promise;

        public PendingMessage(IByteBuf buf, TaskCompletionSource promise)
        {
            Buf = buf;
            Promise = promise;
        }

        internal PendingMessage Success()
        {
            Promise.Success();
            return this;
        }

        internal PendingMessage Return()
        {
            Buf.Return();
            return this;
        }
    }
}
