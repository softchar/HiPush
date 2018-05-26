using Hi.Infrastructure.Base;
using Hi.Infrastructure.Extension;
using Hi.NetWork.Buffer.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Buffer
{
    /*
     * 说明：
     * Arena为了方便申请合适的内存，缓冲Chunk和Page的集合。
     * 如果申请tiny，small大小的内存块，则直接从tinyPoolPage和SmallPoolPages进行查找。
     * 如果没有找到合适的大小，则新建一个chunk，从chunk分配Page，再缓存chunk
     * 
     * 
     * 字节对齐策略 tiny=[0,512),small=[1024,2048,4096,8192],big=[16K...1024*1024 * 16],
     * tiny:最小为16个字节，每次递增16，线性函数为16*n，一直到512，分为32个段
     * small:默认只有4段，1024，2048，4096，8192
     * big:最小为16K，最大16M，后续的字节数成线性增长，线性函数16*2^n
     * 
     */
    public class PoolArena
    {
        static readonly byte TinyNum = 512 >> 4;
        static readonly byte DefaultElemSize = 512 / (512 >> 4);
        static readonly byte SmallNum = 4;
        static readonly int PageSize = 8192;
        static readonly int MaxAllocSize = 8192 * (1 << 11);

        int useables;

        PoolPageList[] tinyPoolPages;
        PoolPageList[] smallPoolPages;

        PoolChunkList ck000;    //scope = [0,  25)      占用0，不足25%
        PoolChunkList ck025;    //scope = [25, 50)      占用25%，不足50%
        PoolChunkList ck050;    //scope = [50, 75)      占用50%，不足75%
        PoolChunkList ck075;    //scope = [75, 100)     占用75%，不足100%
        PoolChunkList ck100;    //scope = [100, 100)    刚好占用100%

        int maxChunkCounter;
        int chunkCounter;
        public int MaxChunkCounter => maxChunkCounter;
        public int ChunkCounter => chunkCounter;
        

        public PoolArena(int maxChunkCounter = 50)
        {
            this.tinyPoolPages = new PoolPageList[TinyNum];
            this.smallPoolPages = new PoolPageList[SmallNum];
            this.maxChunkCounter = maxChunkCounter;

            for (int i = 0; i < tinyPoolPages.Length; i++)
            {
                tinyPoolPages[i] = NewPoolPage((i + 1) * DefaultElemSize);
            }

            for (int i = 0; i < smallPoolPages.Length; i++)
            {
                smallPoolPages[i] = NewPoolPage(512 << (i + 1));
            }
            ck000 = new PoolChunkList(00, 25);
            ck025 = new PoolChunkList(25, 50);
            ck050 = new PoolChunkList(50, 75);
            ck075 = new PoolChunkList(75, 100);
            ck100 = new PoolChunkList(100, 101);

            PoolChunkList.Link(ck000, ck025);
            PoolChunkList.Link(ck025, ck050);
            PoolChunkList.Link(ck050, ck075);
            PoolChunkList.Link(ck075, ck100);

        }

        public IByteBuf Alloc(int size)
        {

            var buf = new FixedLengthByteBuf();

            int newSize = CalcAllocSize(size);

            if (IsTiny(newSize) || IsSmall(newSize))
            {
                AllocPage(buf, newSize, size);
            }
            else
            {
                AllocNode(buf, newSize, size);
            }

            return buf;
        }

        private void AllocPage(IByteBuf buf , int newSize, int size)
        {
            int idx;
            PoolPageList pageList;

            if (IsTiny(newSize))
            {
                idx = (newSize >> 4) - 1;
                pageList = tinyPoolPages[idx];
            }
            else
            {
                idx = IntEx.Log2(newSize >> 10);
                pageList = smallPoolPages[idx];
            }

            var page = pageList.GetNextAvailPage();

            if (page != null)
            {
                long handle = page.Alloc(newSize);
                if (handle > -1)
                {
                    page.Chunk.BytebufInit(buf, handle, newSize, size);
                    useables += newSize;
                }
                return;
            }

            //1，如果缓存中没有elemsize=size的page
            //2，如果从缓冲中分配失败
            //1和2都满足，那么会重新分配一个新的page
            page = AllocNewPage(buf, newSize, size);

            if (page == null) return;

            pageList.AddLast(page);
            
        }

        private void AllocNode(IByteBuf buf, int newSize, int size)
        {
            
        }

        /// <summary>
        /// 使用数
        /// </summary>
        /// <returns></returns>
        public int Useables() => ck000.Useables();

        public PoolPage AllocNewPage(IByteBuf buf, int newSize, int size)
        {
            PoolPage page;
            if (ck050.TryAllocPage(buf, newSize, size, out page)
                || ck025.TryAllocPage(buf, newSize, size, out page)
                || ck000.TryAllocPage(buf, newSize, size, out page)
                || ck075.TryAllocPage(buf, newSize, size, out page)
                || ck100.TryAllocPage(buf, newSize, size, out page))
            {
                useables += newSize;
                return page;
            }

            if (chunkCounter == maxChunkCounter)
            {
                //throw new AllocException($"chunkCounter最大为: { maxChunkCounter }");
                return page;
            }

            var chunk = new PoolChunk(size, this);
            page = chunk.AllocPage(buf, newSize, size);
            this.ck000.AddLast(chunk);
            chunkCounter++;
            useables += newSize;

            return page;
        }

        public PoolPageList NewPoolPage(int elemSize) => new PoolPageList(elemSize);

        /// <summary>
        /// 申请的内存字节对齐
        /// 小于等于512字节数，每次递增16个字节，大于512，字节数按1024*2^n增长
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public int CalcAllocSize(int size)
        {
            int alignSize = CalcAllocSize0(size);
            if (alignSize > MaxAllocSize)
            {
                throw new IndexOutOfRangeException("申请的尺寸必须小于" + MaxAllocSize);
            }
            return alignSize;
        }

        public int CalcAllocSize0(int size)
        {
            if (IsTiny(size))
            {
                if ((size & 15) == 0)
                    return size;
                return (size & ~15) + 16;
            }
            else
            {
                int normalizedCapacity = size;
                normalizedCapacity--;
                normalizedCapacity |= normalizedCapacity >> 1;
                normalizedCapacity |= normalizedCapacity >> 2;
                normalizedCapacity |= normalizedCapacity >> 4;
                normalizedCapacity |= normalizedCapacity >> 8;
                normalizedCapacity |= normalizedCapacity >> 16;
                normalizedCapacity++;

                if (normalizedCapacity < 0)
                {
                    normalizedCapacity = normalizedCapacity >> 1;
                }

                return normalizedCapacity;
            }
        }

        /// <summary>
        /// 小于等于512
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool IsTiny(int size) => (size & 0xFFFFFE00) == 0;

        /// <summary>
        /// 小于等于8192
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool IsSmall(int size) => (size & 0xFFFFE0000) == 0;

        /// <summary>
        /// 释放Return
        /// </summary>
        /// <param name="poolChunk"></param>
        internal void Return(PoolChunk poolChunk)
        {
            
        }
    }
}
