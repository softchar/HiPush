using Hi.NetWork.Eventloops;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Test.Eventloops
{
    [TestClass]
    public class PriorityQueueTest
    {
        [TestMethod]
        public void priority_test()
        {
            var queue = new PriorityQueue<int>();
            for (int i = 0; i < 1024; i++)
            {
                queue.Enqueue(i);
            }

            Assert.AreEqual(queue.Count, 1024);

            for (int i = 0; i < 1024; i++) 
            { 
                Assert.AreEqual(queue.Count, 1024 - i); 

                int item = queue.Dequeue();

                Assert.AreEqual(item, i);
                Assert.AreNotEqual(item, null); 
            }

            Assert.AreEqual(queue.Count, 0);

            int item1 = queue.Dequeue();

            Assert.AreEqual(item1, null);
            
            
        }
    }
}
