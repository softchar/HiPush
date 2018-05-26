using Hi.NetWork.Buffer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Test.ByteBuffer
{
    /* PoolPage默认分配8192个字节数组
     * 
     * 功能列表
     *     1，当第一次请求16个字节后，申请的尺寸就固定为16个字节
     *     以后每次也只能分配16个字节，当申请的大小与固定的尺寸不
     *     同时，返回-1表示申请失败
     *     
     *     2，当可用数为0时，再进行操作，返回-1表示申请失败
     *     
     *     3，释放句柄Handle，返回-1释放失败
     *     
     * 测试列表
     *     1，第一次请求16个字节，第二次再申请一个5个字节，返回-1
     *     表示成功，否则失败
     *     
     *     2，每次申请16个字节，直到Page.CanAlloc=false，再分配5
     *     个字节，返回-1表示成功，否则失败
     *     
     *     3，输入Handle=513)，返回-1表示失败，
     *     输入Handle=512，返回0表示成功
     *     Handle.SegOffset的取值范围在[0,PageSize/ElemSize]之间
     *     默认情况下PageSize/ElemSize=8192/16=512
     *     Handle的格式是前32位存储Chunk.PageIndex,后32位存储SegOffset
     */
    [TestClass]
    public class PoolPageTest
    {
        PoolPage page;
        int elemSize = 16;
        [TestInitialize]
        public void Init()
        {
            page = new PoolPage(null, 0, 8192, elemSize);
        }

        /// <summary>
        /// 1，当第一次请求16个字节后，申请的尺寸就固定为16个字节，以后每次也只能分配16个字节
        /// 当申请的大小与固定的尺寸不同时，返回-1表示申请失败
        /// 
        /// </summary>
        [TestMethod]
        public void poolpage_init_allocsize()
        {
            int defaultCapacity = 8192;
            int allocSize = 16;
            int allocSize2nd = 5;

            Assert.AreEqual(page.Capacity, defaultCapacity);

            long handle = page.Alloc(allocSize);
            
            long errcode = page.Alloc(allocSize2nd);
            if (errcode != -1)
            {
                Console.WriteLine("PoolPage已分配，又重新分配了一个新的尺寸");
                Assert.Fail();
            }
        }

        /// <summary>
        /// 2，当可用数为0时，再进行操作，返回-1表示申请失败
        /// </summary>
        [TestMethod]
        public void poolpage_alloc_bytebuf()
        {
            
            int size2nd = 5;

            long handle;
            for (int i = 1; i <= page.Total && page.CanAlloc; i++)
            {
                handle = page.Alloc(elemSize);
                if (handle == -1)
                {
                    Console.WriteLine("还有可以分配的segment，但是却报告错误了");
                    Console.WriteLine("循环次数:" + i);
                    Console.WriteLine("page.Total:" + page.Total);
                    Console.WriteLine("page.Available:" + page.Available);
                    Console.WriteLine("------------------------------------");
                    Assert.Fail();
                }
            }

            handle = page.Alloc(size2nd);
            
            if (handle == -1 && !page.CanAlloc)
            {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        /// 3，Handle释放
        /// </summary>
        [TestMethod]
        public void poolpage_alloc_canAlloc()
        {

            long handle2 = 1;
            int code;

            //释放一个还没有分配的handle，返回-1
            code = page.Return(handle2);
            Assert.AreEqual(code, -1);

            for (int i = 1; i <= page.Total && page.CanAlloc; i++)
            {
                long handle = page.Alloc(elemSize);
                if (handle == -1)
                {
                    Console.WriteLine("还有可以分配的segment，但是却报告错误了");
                    Console.WriteLine("循环次数:" + i);
                    Console.WriteLine("page.Total:" + page.Total);
                    Console.WriteLine("page.Available:" + page.Available);
                    Console.WriteLine("------------------------------------");
                    Assert.Fail();
                }
            }


            code = 0;
            code = page.Return(handle2);
            Assert.AreEqual(code, 0);

            //将释放的handle再进行释放，将返回-1
            code = 0;
            code = page.Return(handle2);
            Assert.AreEqual(code, -1);

            //释放一个超过范围的值，将返回-1
            code = 0;
            code = page.Return(513);

            Assert.AreEqual(code, -1);

        }

    }
}
