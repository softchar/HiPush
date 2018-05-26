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
     * 测试技巧：
     * 1，先测试基础功能
     * 2，再测试优化的部分
     * （比如功能中使用了二叉堆来优化算法，需要对二叉堆的节点进行验证，最好做随机性测试）
     * 3，函数测试（可选），即测试每一个函数
     * 
     * 说明：
     * PoolArena缓存tiny、small所在的PoolPage和PoolChunk。找到合适的PoolChunk进行分配
     * 
     * 功能列表：
     * 1，
     * 
     */

    [TestClass]
    public class PoolArenaTest
    {
        /// <summary>
        /// 内存字节对齐测试
        /// </summary>
        [TestMethod]
        public void calc_alloc_bytes_test()
        {
            int size1 = 8;
            int size2 = 16;
            int size3 = 1031;
            int size4 = 16589;
            int size5 = 1024 * 1024 * 16 + 1;

            var arena = new PoolArena();
            int s1 = arena.CalcAllocSize(size1);
            Assert.AreEqual(s1, 16);

            int s2 = arena.CalcAllocSize(size2);
            Assert.AreEqual(s2, 16);

            int s3 = arena.CalcAllocSize(size3);
            Assert.AreEqual(s3, 2048);

            int s4 = arena.CalcAllocSize(size4);
            Assert.AreEqual(s4, 32768);

            try
            { 
            	int s5 = arena.CalcAllocSize(size5);
                Assert.Fail();
            } 
            catch (IndexOutOfRangeException ex)
            {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        /// 将poolarena全部分配出去
        /// </summary>
        [TestMethod]
        public void poolarena_alloc_all()
        {
            var chunk = new PoolChunk();
            var arena = new PoolArena(1);

            IByteBuf buf = null;
            int allocBytes = 0;

            do
            {
                buf = arena.Alloc(8192);
                if (buf.Handle != 0)
                {
                    allocBytes += 8192;
                    Assert.AreEqual(allocBytes, arena.Useables());
                }

            } while (buf != null && buf.Handle != 0);

            Assert.AreEqual(allocBytes, chunk.Capacity);
 
        }

        [TestMethod]
        public void poolarena_alloc_all_random()
        {
            //var chunk = new PoolChunk();
            //var arena = new PoolArena(1);

            //IByteBuf buf = null;
            //int allocBytes = 0;

            //int s = 0;
            //int s1 = 0;

            //Random r = new Random();

            //do
            //{
            //    s = r.Next(16, 8192);
            //    s1 = arena.CalcAllocSize(s);

            //    buf = arena.Alloc(s1);
            //    if (buf.Handle != 0)
            //    {
            //        allocBytes += s1;
            //        Assert.AreEqual(allocBytes, arena.Useables());
            //    }

            //} while (buf != null && buf.Handle != 0);

            //Assert.AreEqual(allocBytes, chunk.Capacity);
        }

        [TestMethod]
        public void alloc_subpage()
        {
            var arena = new PoolArena(1);
            var buf1 = arena.Alloc(6);
            var buf2 = arena.Alloc(6);
            var buf3 = arena.Alloc(17);
            var buf4 = arena.Alloc(300);
            Assert.AreEqual(buf1.Offset, 0);
            Assert.AreEqual(buf2.Offset, 16);
            Assert.AreEqual(buf3.Offset, 8192);
            Assert.AreEqual(buf4.Offset, 16384);
        }
    }
}
