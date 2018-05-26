using Hi.Infrastructure.Base;
using Hi.Infrastructure.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Buffer
{
    /// <summary>
    /// Chunk是Page的集合，同时也为快速搜索匹配的Page提供支持
    /// Chunk不维护具体的字节使用情况，分配一个Page就减去一个
    /// page的长度
    /// </summary>
    public class PoolChunk : HiLinkNode<PoolChunk>
    {
        static readonly int DefaultPageSize = 8192;
        static readonly byte DefaultAlign = 16;
        static readonly byte MaxOrder = 11;          //11层的二叉树

        //内存区域
        PoolArena arena;

        //内存页的尺寸
        int pageSize;

        //对齐的字节数
        byte align;

        //二叉堆
        byte[] memoryMap;
        byte[] memoryMapDefault;

        //内存
        byte[] memory;

        //最大容量
        int maxCapacity;

        //节点个数
        int nodeNum;

        //内存页的个数
        int pages;

        //不能分配标记
        byte cannotAssigned;

        //缓冲最后一层的page的集合
        PoolPage[] subpages;

        //已用字节数
        int usedables;

        //是否还可以分配
        bool canAlloc;

        public int Pages => pages;

        /// <summary>
        /// 内存
        /// </summary>
        public Byte[] Memory => this.memory;

        /// <summary>
        /// 所属PoolArena
        /// </summary>
        public PoolArena Arena => arena;

        /// <summary>
        /// 可用字节数
        /// </summary>
        public int Availables => this.maxCapacity - this.usedables;

        /// <summary>
        /// 已用字节数
        /// </summary>
        public int Usedables => usedables;

        /// <summary>
        /// 容量
        /// </summary>
        public int Capacity => maxCapacity;

        /// <summary>
        /// 使用率
        /// </summary>
        public float UsedPercent => 1f * usedables / Capacity;

        /// <summary>
        /// 是否还可以分配
        /// </summary>
        public bool CanAlloc => Availables > 0;


        public PoolChunk()
            : this(DefaultPageSize)
        {

        }

        public PoolChunk(int poolPageSize, PoolArena arena = null)
        {
            this.arena = arena;                                 //chunk所在的arena
            this.pageSize = PageSizeAdjust(poolPageSize);       //修正PageSize，确保PageSize的合法性
            this.align = DefaultAlign;                          //字节对齐
            this.pages = 1 << MaxOrder;                         //poolpage的最大的数量
            this.maxCapacity = pages * pageSize;                //chunk最大容量
            this.cannotAssigned = (byte)(MaxOrder + 1);         //不能分配的标识

            this.subpages = new PoolPage[this.pages];
            this.memory = new byte[this.maxCapacity];
            this.memoryMap = new byte[pages << 1];
            this.memoryMapDefault = new byte[pages << 1];

            int mapIndex = 1;
            for (int i = 0; i <= MaxOrder; i++)
            {
                int depth = 1 << i;
                for (int j = 0; j < depth; j++)
                {
                    memoryMap[mapIndex] = (byte)i;
                    memoryMapDefault[mapIndex] = (byte)i;
                    mapIndex++;
                }
            }
        }

        /// <summary>
        /// 字节对齐
        /// </summary>
        /// <param name="size"></param>
        /// <returns>返回对齐之后的尺寸</returns>
        public int Align(int size)
        {
            return ((size + align - 1) & (~(align - 1)));
        }

        /// <summary>
        /// 分配Page
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="elemSize">Page页的元素尺寸</param>
        /// <param name="size">实际申请的字节数</param>
        /// <returns></returns>
        public PoolPage AllocPage(IByteBuf buf, int elemSize,int size)
        {
            if (!this.CanAlloc)
                return null;

            //如果可用的字节数不足则返回null
            if (this.Availables < size)
                return null;

            PoolPage page = AllocPage(elemSize, size);
            if (page == null) return null;

            //分配一个句柄
            long handle = page.Alloc(elemSize);

            //计算偏移
            int offset = HandleToOffset(handle, elemSize);

            //初始化ByteBuf
            BytebufInit(this.memory, buf, handle, offset, size);

            return page; 

        } 

        /// <summary>
        /// 申请一个Page
        /// </summary>
        /// <param name="elemSize">page元素的尺寸</param>
        /// <param name="size">实际申请的内存尺寸</param>
        /// <returns></returns>
        public PoolPage AllocPage(int elemSize, int size)
        {
            //如果可用的字节数不足则返回null
            if (this.Availables < size)
                return null;

            //计算申请内存尺寸在memoryMap中的深度
            AllocInfo alloc = CalcAllocDepth(size);

            //通过深度获得节点编号
            int id = AllocHandle(alloc.Depth);

            int tableId = PageIdx(id);

            PoolPage page = subpages[tableId];
            if (page == null)
            {
                page = new PoolPage(this, id, alloc.PageCapacity, elemSize);
                subpages[tableId] = page;
            }
            else if(!page.Allocated)
            {
                //找到一个被重置过的Page则重新进行分配
                page.ReAlloc(alloc.PageCapacity, elemSize);
            }

            usedables += pageSize;

            return page;
            
        }

        public int AllocNode(IByteBuf buf, int size)
        {
            return 0;
        }

        /// <summary>
        /// 申请节点
        /// </summary>
        /// <param name="depth"></param>
        /// <returns>返回节点的编号</returns>
        public int AllocHandle(int depth)
        {
            //确保depth不会操作最大的层次
            depth = Math.Min(depth, MaxOrder);

            int id = 1;
            int endid = -(1 << depth);
            byte val = Value(id);

            while (val < depth || (id & endid) == 0)
            {
                id <<= 1;
                val = Value(id);

                //当发现memoryMap对应的值大于需要找的层数，说明该节点已经分配出去了。
                //id ^ 1表示右节点
                if (val > depth)
                {
                    id ^= 1;
                    val = Value(id);
                }
            }

            //将当前节点标记为不能分配
            SetValue(id, cannotAssigned);

            //更新父节点的分配信息
            UpdateParentNodeAllocation(id);

            return id;
        }

        /// <summary>
        /// 初始化ByteBuf
        /// </summary>
        /// <param name="buf">缓冲区容器</param>
        /// <param name="offset">buf在当前字节数组中的偏移</param>
        /// <param name="elemSize">申请内存对齐之后的大小</param>
        /// <param name="capacity">实际申请的大小</param>
        internal static void BytebufInit(Byte[] memory, IByteBuf buf, long handle, int offset, int capacity)
        {
            buf.SetBytes(memory);
            buf.SetHandle(handle);
            buf.SetOffset(offset);
            buf.SetCapacity(capacity);
        }

        internal void BytebufInit(IByteBuf buf, long handle, int newSize, int capacity)
        {
            int offset = HandleToOffset(handle, newSize);
            BytebufInit(this.memory, buf, handle, offset, capacity);
        }

        /// <summary>
        /// 释放handle，当length=PageSize则表示释放整个Page的长度
        /// 否则只释放length长度
        /// 
        /// 异常：
        ///     IndexOutOfRangeException：如果handle不合法抛出异常
        ///     IndexOutOfRangeException
        /// 
        /// </summary>
        /// <param name="handle">需要释放的句柄</param>
        /// <param name="length">需要释放的长度</param>
        public void Return(int handle)
        {
            ValidHandle(handle);

            //将当前节点的值还原
            SetValue(handle, ValueDefaut(handle));

            //更新父节点
            UpdateParentNodeAllocation(handle);

            PoolPage page = subpages[PageIdx(handle)];
            page.Clear();

            usedables -= pageSize;

            //如果chunk空了。
            if (usedables == 0)
            {
                Return();
                arena.Return(this);
            }
            
        }

        /// <summary>
        /// 释放整个Chunk
        /// </summary>
        public void Return()
        {
            memoryMap = null;
            memory = null;
            memoryMapDefault = null;
            subpages = null;

            usedables = 0;
        }

        /// <summary>
        /// 获得PageIndex
        /// 2048^2048=0    =2048-2048
        /// 2049^2048=1    =2049-2048
        /// 2050^2048=2    =2050-2048
        /// 2051^2048=3    =2051-2048
        /// 4095^2048=2047 =4095-2048
        /// X^Y，当Y=2^n，X∈[Y,2Y)时，X^Y=X-Y
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        private int PageIdx(int nodeId) => nodeId ^ pages;

        /// <summary>
        /// 偏移的计算方式
        /// PageOffset * PageSize + SegOffset * ElemSize
        ///    PageOffset:页偏移
        ///    PageSize:每页所占的字节
        ///    SegOffset(Segment Offset):段在页中的偏移
        ///    ElemSize(Element Size):段元素的尺寸
        /// </summary>
        /// <param name="handle">申请内存段时获得的Handle</param>
        /// <param name="elemSize">申请内存段时进行内存对齐之后的字节数</param>
        /// <returns></returns>
        private int HandleToOffset(long handle, int elemSize) => PageIdx(HandleToPageIndex(handle)) * pageSize + HandleToSegIndex(handle) * elemSize;

        /// <summary>
        /// 将Handle转换为PageIndex
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        private int HandleToPageIndex(long handle) => (int)(handle >> 32);

        /// <summary>
        /// 将Handle转换为SegIndex(段的索引)
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        private int HandleToSegIndex(long handle) => (int)handle;

        /// <summary>
        /// 更新父节点的分配信息
        /// </summary>
        /// <param name="id"></param>
        private void UpdateParentNodeAllocation(int id)
        {
            while (id > 0)
            {
                int parentId = id >> 1;

                if (parentId == 0) break;

                //子节点
                byte valL = Value(id);

                //兄弟节点
                byte valR = Value(id ^ 1);

                byte val;
                if (valL == valR && valL == ValueDefaut(id))
                {
                    val = ValueDefaut(parentId);
                }
                else
                {
                    //val大于原始值，表示已分配，所以取小。
                    val = Math.Min(valL, valR);
                }

                SetValue(parentId, val);

                id = parentId;
            }
        }

        /// <summary>
        /// 给定一个尺寸，计算在memoryMap中深度，最大值为MaxOrder
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public AllocInfo CalcAllocDepth(int size)
        {
            if (size > this.maxCapacity)
            {
                throw new IndexOutOfRangeException($"size必须小于等于最大容量{ this.maxCapacity }");
            }

            //如果size<PageSize那么直接返回最大的层次
            if (size < pageSize)
            {
                return new AllocInfo() { Depth = (byte)MaxOrder, PageCapacity = pageSize };
            }

            int value = pageSize;

            byte d = 0;
            while (value < size)
            {
                value <<= 1;
                d++;
            }

            return new AllocInfo() { Depth = (byte)(MaxOrder - d), PageCapacity = value };
        }

        /// <summary>
        /// 通过编号获得对应memoryMap中的值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected byte Value(int id)
        {
            return memoryMap[id];
        }
        protected byte ValueDefaut(int id)
        {
            return memoryMapDefault[id];
        }

        private void SetValue(int id, byte value)
        {
            memoryMap[id] = value;
        }

        /// <summary>
        /// 调整内存页的尺寸
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static int PageSizeAdjust(int pageSize)
        {

            //for (int i = 0; i < pageSizes.Length; i++)
            //{
            //    if (pageSizes[i] >= pageSize)
            //        return pageSizes[i];
            //}

            return DefaultPageSize;

            //for (int i = 2; i < 17; i++)
            //{
            //    int t = (int)Math.Pow(2, i);
            //    if (t > pageSize)
            //    {
            //        return t;
            //    }
            //}

        }

        /// <summary>
        /// 验证handle的合法性
        /// </summary>
        /// <param name="handle"></param>
        private void ValidHandle(long handle)
        {
            int pageIndex = HandleToPageIndex(handle);
            int segIndex = HandleToSegIndex(handle);

            PoolPage page;
            try
            {
                page = subpages[PageIdx(pageIndex)];

                if (page == null || segIndex < 0 || segIndex >= page.Capacity)
                    throw new IndexOutOfRangeException();
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException("handle的值不合法");
            }
        }
        private void ValidHandle(int handle)
        {
            
            try
            {
                PoolPage page = subpages[PageIdx(handle)];

                if (page == null)
                    throw new IndexOutOfRangeException();
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException("handle的值不合法");
            }
        }

        public struct AllocInfo
        {
            public byte Depth;
            public int Size;
            public int PageCapacity;
        } 

    }
}
