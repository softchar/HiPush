using Hi.NetWork.Buffer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Test.ByteBuffer
{
    /*
     * 功能列表
     * 1，当chunk的使用范围不在当前的List之中时，会进行转移
     * 
     * 测试列表
     * 1，新建两个chunklist并链接，新建一个chunk添加到小的中，当chunk使用率超过
     * 小chunk中的范围时，会向下转移
     * 
     */ 

    [TestClass]
    public class PoolChunkListTest
    {
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void chunklist_move()
        {
            var chunk1 = new PoolChunk();
            var chunk2 = new PoolChunk();

            var chunklist1 = new PoolChunkList(0, 25);
            var chunklist2 = new PoolChunkList(25, 50);
            PoolChunkList.Link(chunklist1, chunklist2);

            chunklist1.AddLast(chunk1);

            for (int i = 0; chunk1.CanAlloc ; i++)
            {
                var buf = new FixedLengthByteBuf();

                PoolPage page;

                int s = 16 * (i + 1);
                if (s >= 8192) s = 8192;

                chunklist1.TryAllocPage(buf, s, s, out page);

                if (page == null)
                {
                    Assert.Fail();
                }

                if (chunk1.UsedPercent < 0.25)
                {
                    Assert.AreNotEqual(chunklist1.Head, null);
                    Assert.AreEqual(chunklist2.Head, null);
                }
                else
                {
                    Assert.AreEqual(chunklist1.Head, null);
                    Assert.AreNotEqual(chunklist2.Head, null);
                    break;
                }
            }

        }

        /// <summary>
        /// 新建一个chunklist，范围在[0-100]
        /// 新建两个chunk1和chunk2，分别添加到chunkList之中
        /// 通过chunkList将chunk1的内存全部分配出去，使得chunk1.CanAlloc=false，
        /// 再进行分配，chunkList会使用chunk2进行分配
        /// </summary>
        [TestMethod]
        public void chunklist_next()
        {
            var chunklist = new PoolChunkList(0, 100);

            var chunk1 = new PoolChunk();
            var chunk2 = new PoolChunk();

            chunklist.AddLast(chunk1);
            chunklist.AddLast(chunk2);

            int s = 0;

            for (int i = 0; chunk1.CanAlloc; i++)
            {
                s = 16 * (i + 1);
                if (s >= 8192) s = 8192;

                PoolPage page;
                var buf = new FixedLengthByteBuf();
                if (!chunklist.TryAllocPage(buf, s, s, out page))
                {
                    Assert.Fail();
                }

                Assert.AreEqual(page.Chunk, chunk1);
            }

            PoolPage page1;
            var buf1 = new FixedLengthByteBuf();
            if (!chunklist.TryAllocPage(buf1, s, s, out page1))
            {
                Assert.Fail();
            }

            Assert.AreEqual(page1.Chunk, chunk2);


        }
    }
}
