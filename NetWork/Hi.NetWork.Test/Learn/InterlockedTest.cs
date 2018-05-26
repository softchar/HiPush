using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Test.Learn {
    [TestClass]
    public class InterlockedTest {

        [TestMethod]
        public void chanpange() {
            int a = 0;
            var result = System.Threading.Interlocked.CompareExchange(ref a, 1, 0);

            int b = 1;
            var result2 = System.Threading.Interlocked.CompareExchange(ref b, 1, 0);
            
        } 

    }
}
