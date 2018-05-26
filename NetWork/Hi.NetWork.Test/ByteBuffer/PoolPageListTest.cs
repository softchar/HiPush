using Hi.NetWork.Buffer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Test.ByteBuffer
{
    [TestClass]
    public class PoolPageListTest
    {
        [TestMethod]
        public void poolpagelist_()
        {
            var chunk = new PoolChunk();
            var pageList = new PoolPageList(16);

            var page1 = chunk.AllocPage(8192, 8192);
            var page2 = chunk.AllocPage(8192, 8192);

            pageList.AddLast(page1);
            pageList.AddLast(page2);

            //获得下一个可用的PoolPage
            var _page1 = pageList.GetNextAvailPage();
            Assert.AreEqual(page1, _page1);
            _page1.Alloc(8192);

            var _page2 = pageList.GetNextAvailPage();
            Assert.AreEqual(page2, _page2);

        }

        /// <summary>
        /// 功能列表：
        /// 1，当pagelist的数量为1时，删除第一个之后head=null并且tail=null
        /// </summary>
        [TestMethod]
        public void poolpageList_deletefirst()
        {
            var chunk = new PoolChunk();
            var pagelist = new PoolPageList(8192);
            Assert.AreEqual(pagelist.Head, null);
            Assert.AreEqual(pagelist.Tail, null);
            Assert.AreEqual(pagelist.Count, 0);

            var page1 = chunk.AllocPage(8192, 8192);
            var page2 = chunk.AllocPage(8192, 8192);

            pagelist.AddLast(page1);
            Assert.AreEqual(pagelist.Count, 1);
            Assert.AreEqual(pagelist.Head, page1);
            Assert.AreEqual(pagelist.Tail, page1);

            pagelist.DeleteFirst();
            Assert.AreEqual(pagelist.Count, 0);
            Assert.AreEqual(pagelist.Head, null);
            Assert.AreEqual(pagelist.Tail, null);

            pagelist.AddLast(page1);
            Assert.AreEqual(pagelist.Count, 1);
            Assert.AreEqual(pagelist.Head, page1);
            Assert.AreEqual(pagelist.Tail, page1);

            pagelist.AddLast(page2);
            Assert.AreEqual(pagelist.Count, 2);
            Assert.AreEqual(pagelist.Head, page1);
            Assert.AreEqual(pagelist.Tail, page2);

            pagelist.DeleteFirst();
            Assert.AreEqual(pagelist.Count, 1);
            Assert.AreEqual(pagelist.Head, page2);
            Assert.AreEqual(pagelist.Tail, page2);


        }

        [TestMethod]
        public void poolpageList_deletefirst_mul()
        {
            var chunk = new PoolChunk();
            var pagelist = new PoolPageList(8192);
            Assert.AreEqual(pagelist.Head, null);
            Assert.AreEqual(pagelist.Tail, null);

            int s = 0;
            var pages = new List<PoolPage>();
            for (int i = 0; chunk.CanAlloc; i++)
            {
                s = (i + 1) * 16;
                if (s > 8192) s = 8192;
                var page = chunk.AllocPage(s, s);

                pages.Add(page);

                pagelist.AddLast(page);
            }

            for (int i = 0; i < pages.Count; i++)
            {
                var page1 = pagelist[i];
                var page2 = pages[i];
                Assert.AreEqual(page1, page2);
            }
            
            Assert.AreEqual(pages.Count, pagelist.Count);

            Console.WriteLine(pagelist.Count);

            try
            {
                var page2049 = pagelist[pages.Count];
                Assert.Fail();
            }
            catch(IndexOutOfRangeException)
            {
                Assert.IsTrue(true);
            }
            



        }
    }
}
