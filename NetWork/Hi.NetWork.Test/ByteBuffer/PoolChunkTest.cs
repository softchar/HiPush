using Hi.NetWork.Buffer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Hi.NetWork.Buffer.PoolChunk;
using Hi.Infrastructure.Extension;
using System.Diagnostics;

namespace Hi.NetWork.Test.ByteBuffer
{
    /* 默认值
     *     PoolChunk.PageSize = 8192
     *     PoolChunk.MaxOrder = 11
     *     PoolChunk.AvailableBytes = (1 << 11) * 8192
     * 
     * 功能列表
     * PoolChunk主要负责Page的分配，Chunk维护一个二叉堆，索引表示树的节点
     * 值表示节点在树上的深度
     *     
     *     1，申请一个Page，申请失败返回null
     *     
     *     2，将一个Chunk的内存全部分配出去，再强行申请内存，返回null
     *     
     *     3，释放句柄，成功返回0，失败返回-1，如果是一个无效的句柄，
     *     抛出异常IndexOutOfRangeException
     *     
     * 测试列表
     * 
     *     1，申请一个容量（Capacity）为8192，分配元素尺寸（ElemSize）为
     *     16的Page
     *     
     *     成功：Page != null，Page.Capacity=8192，Page.ElemSize=16
     *     
     *     失败：Page == null
     *     
     *     
     *     2，一次申请分配6个字节，直到Chunk的AvailableBytes=0为止
     *     
     *     成功：Chunk.AvailableBytes == 0，Chunk.UsedAbleBytes == 0
     *     
     *     失败：当Chunk.CanAlloc == true时，返回的Page==Null则失败
     *     当Chunk.CanAlloc == false时，Chunk.AvailableBytes != 0，
     *     Chunk.UsedAbleBytes != 0 则失败
     *     
     *     2.2，按照随机数进行分配，直到Chunk的AvailableBytes=0为止，目
     *     的是模拟真实场景，随机数范围[16,8192]。随机数会进行字节对齐，
     *     按照tiny，smal的方式进行对齐
     *     
     *     tiny：最小为16，依次按16递增至512
     *     small：[1024,2048,4096,8192]
     *     
     *     成功：Chunk.AvailableBytes == 0，Chunk.UsedAbleBytes == 0
     *     
     *     失败：当Chunk.CanAlloc == true时，返回的Page==Null则失败
     *     当Chunk.CanAlloc == false时，Chunk.AvailableBytes != 0，
     *     Chunk.UsedAbleBytes != 0 则失败
     *     
     *     3，从PoolChunk中申请一个Page，Page==null则失败。
     *     执行Page1.Alloc申请一个句柄，句柄不等于-1，否则失败。
     *     chunk.UsedAbleBytes=8192，否则失败。
     *     chunk.Reteurn释放句柄，返回0表示成功，异常则失败。
     *     释放一个无效的句柄，捕获IndexOutOfRangeException成功，否则失败。    
     *     
     */
    [TestClass]
    public class PoolChunkTest
    {
        int elemSize;
        int size;
        int pageSize;
        int maxorder;
        int capacity;

        PoolChunk chunk;

        [TestInitialize]
        public void Init()
        {
            elemSize = 16;
            size = 6; 
            pageSize = 8192;
            maxorder = 11;
            capacity = (1 << maxorder) * 8192;
            chunk = new PoolChunk(pageSize, null);

            Assert.AreEqual(chunk.Capacity, capacity);
            Assert.AreEqual(chunk.Availables, capacity);
            Assert.AreEqual(chunk.Usedables, 0);
        }

        /// <summary>
        /// 字节对齐测试
        /// </summary>
        [TestMethod]
        public void poolchunk_bytealign()
        {
            var chunk = new PoolChunk();

            int aligned1 = chunk.Align(6);
            int aligned2 = chunk.Align(20);

            Assert.AreEqual(aligned1, 16);
            Assert.AreEqual(aligned2, 32);

        }

        /// <summary>
        /// 1，申请一个容量（Capacity）为8192，分配元素尺寸（ElemSize）为
        ///    16的Page
        ///
        /// 成功：Page != null，Page.Capacity=8192，Page.ElemSize=16
        ///     
        /// 失败：Page == null
        /// </summary>
        [TestMethod]
        public void poolchunk_allocPage()
        {
            PoolPage page = chunk.AllocPage(elemSize, size);

            Assert.AreNotEqual(page, null);
            Assert.AreEqual(page.Capacity, pageSize);
            Assert.AreEqual(page.ElemSize, elemSize);
            Assert.AreEqual(chunk.Availables, capacity - pageSize );
            Assert.AreEqual(chunk.Usedables, pageSize);
            Assert.AreEqual(chunk.CanAlloc, true);
        }

        /// <summary>
        /// 2，一次申请分配6个字节，直到Chunk的AvailableBytes=0为止
        ///     
        /// 成功：当Chunk.Canalloc==true时，返回的Page!=null且
        /// Chunk.AvailableBytes!=0，Chunk.UsedAbleBytes!=0
        /// 
        /// 失败：当Chunk.CanAlloc==true时，返回的Page==null，
        /// 当Chunk.CanAlloc == false时，Chunk.AvailableBytes != 0，
        /// Chunk.UsedAbleBytes != 0
        /// </summary>
        [TestMethod]
        public void poolchunk_allocAllPage()
        {
            int i = 0;
            int useables = 0;

            for (; chunk.CanAlloc; i++)
            {
                Assert.AreNotEqual(chunk.Availables, 0);
                
                PoolPage page0 = chunk.AllocPage(elemSize, 6);
                useables += pageSize;

                Assert.AreNotEqual(chunk.Usedables, 0);
                Assert.AreEqual(chunk.Availables, capacity - useables);
                Assert.AreEqual(chunk.Usedables, useables);
            }

            PoolPage page1 = chunk.AllocPage(elemSize, 6);
            Assert.AreEqual(page1, null);
            Assert.AreEqual(chunk.Availables, 0);
            Assert.AreEqual(chunk.Usedables, capacity);
        }

        /// <summary>
        /// 3，从PoolChunk中申请一个Page，Page==null则失败。
        /// 执行Page1.Alloc申请一个句柄，句柄不等于-1，否则失败。
        /// chunk.UsedAbleBytes=8192，否则失败。
        /// chunk.Reteurn释放句柄，返回0表示成功，异常则失败。
        /// 释放一个无效的句柄，捕获IndexOutOfRangeException成功，否则失败。
        /// </summary>
        [TestMethod]
        public void poolchunk_return()
        {
            PoolPage page = chunk.AllocPage(elemSize, size);
            Assert.AreNotEqual(page, null);

            long handle = page.Alloc(elemSize);
            
            Assert.AreNotEqual(handle, -1);
            Assert.AreEqual(chunk.Usedables, pageSize);

            chunk.Return(page.Handle);

            Assert.AreEqual(chunk.Usedables, 0);
            Assert.IsFalse(page.Allocated);
            Assert.AreEqual(page.ElemSize, 0);

            try
            {
            	chunk.Return(page.Handle + 1);
                Assert.Fail();
            }
            catch (IndexOutOfRangeException ex)
            {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        /// 随机分配大小，一直到Chunk不能分配为止
        /// </summary>
        [TestMethod]
        public void poolchunk_allocAllPage_random()
        {
            Random rd = new Random();

            int val;
            int useables = 0;

            for (int i = 0; chunk.CanAlloc ; i++)
            {
                val = rd.Next(elemSize, pageSize);
                int size = CalcAllocSize(val);

                Assert.AreNotEqual(chunk.Availables, 0);

                PoolPage page = chunk.AllocPage(size, val);

                useables += pageSize;

                Assert.AreNotEqual(chunk.Usedables, 0);
                Assert.AreEqual(chunk.Availables, capacity - useables);
                Assert.AreEqual(chunk.Usedables, useables);
            }

            val = rd.Next(16, 8192);
            int size2 = CalcAllocSize(val);

            PoolPage page1 = chunk.AllocPage(size2, val);

            Assert.AreEqual(page1, null);
            Assert.AreEqual(chunk.Availables, 0);
            Assert.AreEqual(chunk.Usedables, capacity);
        } 

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void poolchunk_CalcAllocDepth_Test()
        {
            int size1 = 10;
            int size2 = 8193;
            int size3 = 8192 * 2 + 1;
            int size4 = 8192 * 10000;

            var chunk = new PoolChunk();

            AllocInfo ai1 = chunk.CalcAllocDepth(size1);
            AllocInfo ai2 = chunk.CalcAllocDepth(size2);
            AllocInfo ai3 = chunk.CalcAllocDepth(size3);
            

            Assert.AreEqual(ai1.Depth, 11);
            Assert.AreEqual(ai2.Depth, 10);
            Assert.AreEqual(ai3.Depth, 9);

            try
            {
                AllocInfo ai4 = chunk.CalcAllocDepth(size4);
                Assert.Fail();
            }
            catch (IndexOutOfRangeException ex)
            {
                Assert.IsTrue(true);
                Console.WriteLine(ex.Message);
            }
        }

        private void hanot(int n, string x, string y, string z)
        {
            if (n == 1)
            {
                move(x, z);
            }
            else
            {
                hanot(n - 1, x, z, y);
                hanot(1, x, y, z);
                hanot(n - 1, y, x, z);
            }


        }

        private void move(string l1, string l2)
        {
            Console.WriteLine($"{l1}->{l2}");
        }


        public int CalcAllocSize(int size)
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
    }
}
