using Hi.Infrastructure.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Buffer
{
    /*
     * 对象池的释放策略，一旦空闲列表占全部对象总数量的85%则进行缩小
     * 缩小策略，每次缩小空闲列表的20%
     * 
     */ 

    /// <summary>
    /// 固定长度字节缓冲区池
    /// </summary>
    public class ThreadLocalByteBufPool : HiThreadLocal<ThreadLocalByteBufPool>, IBytebufPool
    {
        //Pool的最大值
        static int DefaultMaxCounter = 262144;

        //Pool的最小值
        static int DefaultMinCounter = 256; //256*1024=262144

        ThreadLocalPooledByteBufConfig config = new ThreadLocalPooledByteBufConfig();

        Queue<IByteBuf> freeStack = new Queue<IByteBuf>();
        Stack<IByteBuf> bufStack = new Stack<IByteBuf>(DefaultMinCounter);
        HashSet<IByteBuf> references = new HashSet<IByteBuf>();
        
        /// <summary>
        /// buffer计数的最大值
        /// </summary>
        public int MaxBufferCounter
        {
            get { return config.MaxBufferCounter; }
        }

        /// <summary>
        /// 可用的数量
        /// </summary>
        public int Available
        {
            get
            {
                //if (queue == null) return 0;
                //return queue.Count;
                return 0;
            }
        }

        /// <summary>
        /// 是否不可用（当Available=0时IsNotAvailable=True）
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return Available > 0;
            }
        }

        /// <summary>
        /// 引用次数（已经使用的ByteBuf的个数）
        /// </summary>
        public int References
        {
            get 
            {
                //if (referenceSet == null) return 0;
                //return referenceSet.Count;
                return 0;
            }
        }

        /// <summary>
        /// 总计数
        /// </summary>
        public int Count
        {
            get { return Available + References; }
        }

        int capacity;

        /// <summary>
        /// 创建ByteBuf处理函数
        /// </summary>
        /// <remarks>
        /// 当T对象需要更复杂的构造时，create handle能提供更好的支持
        /// </remarks>
        public Func<IByteBuf> NewByteBufHandle { get; set; } = () => new PooledByteBuf();

        public ThreadLocalByteBufPool()
        {
            freeStack = new Queue<IByteBuf>();
            bufStack = new Stack<IByteBuf>(DefaultMinCounter);
            references = new HashSet<IByteBuf>();
        }

        protected override ThreadLocalByteBufPool Initialize()
        {
            for (int i = 0; i < DefaultMinCounter; i++)
            {
                IByteBuf buf = NewByteBufHandle();
                bufStack.Push(buf);
                references.Add(buf);
            }
            return this;
        }

        public IByteBuf Get()
        {
            IByteBuf result;

            //TryExtension();

            //先从释放栈获取
            result = freeStack.FirstOrDefault();
            if (result != null)
            {
                result = freeStack.Dequeue();
                return result;
            }

            //如果释放栈没有可用的buf，从默认的池内获取
            result = bufStack.FirstOrDefault();
            if (result != null)
            {
                result = bufStack.Pop();
                return result;
            }

            return null;
        }

        public bool Return(IByteBuf buf)
        {
            bool result = false;
            result = references.Contains(buf);
            freeStack.Enqueue(buf);
            return result;
        }
    }

    public class ThreadLocalPooledByteBufConfig
    {
        //最大的缓冲区的个数
        public int MaxBufferCounter = 200000; 

        //初始缓冲区的个数
        public int BufferCounter = 1024; 

        //增量（当buffer的数量不够了时，每次增加1024个）
        public int BufferIncrement = 1024; 

        //缓冲区的大小
        public int BufferSize = 4096; 
        
    }
}
