using Hi.Infrastructure.Base;
using Hi.Infrastructure.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Buffer
{
    /*
     * bitmap中，1表示未分配，0表示已分配
     */ 

    /// <summary>
    /// 内存页
    /// </summary>
    public class PoolPage : HiLinkNode<PoolPage>, IComparable<PoolPage>
    {
        static readonly int DefaultCapacity = 8192;

        PoolChunk chunk;
        int handle;
        int capacity;
        int elemSize;
        int available;
        int total;
        long[] bitmap;

        /// <summary>
        /// 容量
        /// </summary>
        public int Capacity => capacity;

        /// <summary>
        /// 是否已分配(是否已经确定了分配的尺寸)
        /// </summary>
        public bool Allocated => elemSize > 0;

        /// <summary>
        /// 已用数
        /// </summary>
        public int Used => total - available;

        /// <summary>
        /// 可用数
        /// </summary>
        public int Available => available;

        /// <summary>
        /// 已使用的字节数
        /// </summary>
        public int Useable => Used * elemSize;

        /// <summary>
        /// 总数
        /// </summary>
        public int Total => total;

        /// <summary>
        /// 当前的Page对应Chunk中的节点
        /// </summary>
        public int Handle => handle;

        /// <summary>
        /// 还可以分配
        /// </summary>
        public bool CanAlloc => !Allocated || Available > 0;

        /// <summary>
        /// 元素尺寸
        /// </summary>
        public int ElemSize => elemSize;

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsEmpty => Used == 0;

        /// <summary>
        /// Page所属的Chunk
        /// </summary>
        public PoolChunk Chunk => chunk;

        public IByteBuf NewObjectFactory(int capacity) => new FixedLengthByteBuf(capacity);

        public PoolPage()
            : this(null, 0, DefaultCapacity, 0)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunk">Page所属的PoolChunk</param>
        /// <param name="handle">Page在PoolChunk中的节点编号</param>
        /// <param name="capacity">Page的容量</param>
        /// <param name="size">需要分配的尺寸</param>
        public PoolPage(PoolChunk chunk, int handle ,int capacity, int elemSize)
        {
            this.chunk = chunk;
            this.handle = handle;
            this.capacity = capacity;
            this.elemSize = elemSize;
            total = elemSize == 0 ? 0 : capacity / elemSize;

            if (elemSize > 0)
            {
                Init();
            }
        }

        /// <summary>
        /// 返回一个long类型的Handle(句柄)
        /// handle的作用是用来计算申请的内存块在byte数组中的偏移
        /// 高32位表示当前Page在Chunk中的Handle
        /// 低32位表示分配的段是Page中的偏移（第几个,从0开始）
        /// 
        /// 如果
        /// 
        /// </summary>
        /// <param name="size">需要分配的</param>
        /// <returns>-1表示分配失败</returns>
        public long Alloc(int size)
        {
            if (!ValidAllocSize(size)) return -1;

            //如果还没有分配则设置分配的尺寸
            if (!Allocated)
            {
                this.elemSize = size;
                Init();
            }

            if (size != elemSize || available == 0)
            {
                return -1;
            }

            //获得一个Handle(句柄)并且更新位图
            int bit = GetHandleAndUpdateBitMap();

            if (bit == -1) return -1;

            available--;

            long handle0 = ToHandle(handle, bit);

            return handle0;

        }

        private void Init()
        {
            int len = total >> 6;
            if (len == 0)
            {
                len = 1;
            }
            this.bitmap = new long[len];
            for (int i = 0; i < len; i++)
            {
                //设置为-1L，表示将所有的位全部置为1
                bitmap[i] = -1L;
            }
            this.available = Total;
        }

        /// <summary>
        /// 计算Handle，高32位表示pageHandle，低32位表示在当前page的偏移
        /// </summary>
        /// <param name="pagehandle"></param>
        /// <param name="size"></param>
        /// <returns>高32位表示pageHandle，低32位表示在当前page的偏移</returns>
        public static long ToHandle(long pagehandle, long size)
        {
            return pagehandle << 32 | size;
        }

        public static int ToSegOffset(long handle)
        {
            return (int)handle; 
        }

        /// <summary>
        /// 更新bitmap，并返回当前的
        /// </summary>
        /// <returns></returns>
        private int GetHandleAndUpdateBitMap()
        {
            //
            //方法1：先遍历数组，再遍历位
            //优点：思路简单
            //缺点：极端条件下最高可能遍历512次
            //
            //for (int i = 0; i < bitmap.Length; i++)
            //{
            //    for (int j = 0; j < 64; j++)
            //    {
            //        //注意是1L不是1
            //        long mask = 1L << j;
            //        if ((bitmap[i] & mask) == 0)
            //        {
            //            bitmap[i] |= mask;
            //            return (i << 6) + j;  //i*64+j
            //        } 
            //    }
            //}
            //return -1;

            //
            //方法2：采用位运算
            //优点：最多只需要遍历8次就能获得结果
            //缺点：不易理解
            //
            int result = -1;

            for (int i = 0; i < bitmap.Length; i++)
            {
                long b = bitmap[i];
                if (b != 0)
                {
                    /*
                     * 获得当前long的第一个1开始的值 
                     * 比如：
                     *     10010001，计算得到1
                     *     10010010，计算得到10
                     *     10010100，计算得到100
                     */
                    long mask = b & ~(b - 1);

                    /*
                     * 关闭位
                     * 比如：bitmap[i]=10010100，mask=00000100
                     *     10010100 ^ 00000100 = 10010000 将第2号位的1置0
                     */ 
                    bitmap[i] ^= mask;

                    /*
                     * 为了方便传值，我们使用一个long类型来携带分配的信息
                     * (更清晰简单的办法是直接返回mask和索引i),这里我们将
                     * mask和i浓缩到一个long类型中来传递
                     * 
                     * 根据掩码mask和索引i计算出具体的位数，因为long型的
                     * mask只有一个1，所以我们只需要得到值为1的序号就OK了
                     */ 
                    
                    int h32 = (int)(mask >> 32);        //高32位的值
                    int l32 = (int)mask;                //低32位的值
                    
                    //如果1在高位
                    if (h32 > 0)
                    {
                        //在高位时需要+32
                        //i<<6 = i*64 表示在第几号long中
                        return IntEx.Log2(h32) + 32 + (i << 6);
                    }
                    //如果1在低位
                    else
                    {
                        return IntEx.Log2(l32) + (i << 6);
                    }
                }
            }

            return result;
        }

        internal void ReAlloc(int pageCapacity, int elemSize)
        {
            this.elemSize = elemSize;
            this.total = elemSize == 0 ? 0 : capacity / elemSize;

            if (elemSize > 0)
            {
                Init();
            }
        }

        /// <summary>
        /// 确认分配的尺寸
        /// </summary>
        private bool ValidAllocSize(int size) => !(Allocated && size != elemSize || size > capacity);

        /// <summary>
        /// 验证Handle的合法性，只验证Handle的低32位，高32位由chunk验证
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        private bool ValidHandle(long handle)
        {
            int segOffset = ToSegOffset(handle);
            if (segOffset < 0 || segOffset > Total)
                return false;
            return true;
        }

        /// <summary>
        /// 确保可以分配(可分配=已经初始化+还有可用的项)
        /// </summary>
        private void EnsureCanAlloc()
        {
            if (!Allocated)
            {
                throw new AllocSizeException();
            }
            if (!CanAlloc)
            {
                throw new AllocNoItemException();
            }
        }

        /// <summary>
        /// 释放句柄
        /// </summary>
        /// <param name="handle"></param>
        /// <returns>0：表示成功；-1：失败</returns>
        public int Return(long handle)
        {
            //验证Handle的合法性，不合法返回-1
            if (!ValidHandle(handle)) return -1;

            //段偏移
            int segOffset = ToSegOffset(handle);

            //段在Bitmap中的表的索引
            int segIdx = segOffset >> 6;

            //获得段所在的表
            long table = bitmap[segIdx];

            //计算段所在表中位
            int bit = segOffset - (segIdx << 6);

            //打开位掩码
            long mask = 1L << bit;

            //如果当前位为1表示未分配，返回-1
            if ((table & mask) == mask)
            {
                return -1;
            }

            //将所在位标记为0表示释放
            bitmap[segIdx] ^= mask;


            Return0(handle);

            return 0;
            
        }

        public void Return()
        {
            Clear();

            Return0(0);
            
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Clear()
        {
            bitmap = null;
            available = total = 0;
            elemSize = 0;
        }

        /// <summary>
        /// 释放内存字节
        /// </summary>
        /// <param name="handle"></param>
        private void Return0(long handle)
        {
            if (IsEmpty)
            {
                chunk.Return(this.handle);
            }
        } 

        public int CompareTo(PoolPage other)
        {
            if (other == null)
                return -1;
            return this.GetHashCode().CompareTo(other.GetHashCode());
        }
    }
}
