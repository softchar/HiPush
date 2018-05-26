using Hi.NetWork.Buffer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hi.NetWork.Test.ByteBuffer
{
    /*
     * 需求列表：同一个线程下，new两个相同的对象，
     * 
     */ 

    [TestClass]
    public class HiThreadLocalTest
    {
        [TestMethod]
        public void T1()
        {
            var stack = new Stack<int>();
            stack.Push(1);
            stack.Push(2);
            var queue = new Queue<int>();
            var s = stack.FirstOrDefault();
            if (s != 0)
            {
                s = stack.Pop();
            }
            var q = queue.Dequeue();
            

            HiThreadLocal1.NewObjectFactory = () => new HiThreadLocal1(1);

            var h1 = HiThreadLocal1.Value;
            var h2 = HiThreadLocal1.Value;

            Assert.AreEqual(h1, h2);

            h2.SetValue(2);

            Assert.AreEqual(h1.value, 2);

            HiThreadLocal1 h3 = null;
            HiThreadLocal1 h4 = null;

            var t1 = new Thread(()=> 
            {
                h3 = HiThreadLocal1.Value;
            });
            var t2 = new Thread(() => 
            {
                h4 = HiThreadLocal1.Value;
            });
            
            t1.Start();
            t2.Start();

            Thread.Sleep(2000);

            Assert.AreNotEqual(h3, h4);
        }
    }

    public class HiThreadLocal1 : HiThreadLocal<HiThreadLocal1>
    {
        public int value = 0;

        public HiThreadLocal1()
        {
            
        }

        public HiThreadLocal1(int val)
        {
            this.value = val;
        }

        protected override HiThreadLocal1 Initialize()
        {
            return this;
        }

        public void SetValue(int v)
        {
            this.value = v;
        }
    }
}
