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
    public class PooledThreadLocalBytebufAlloctorTest
    {
        [TestMethod]
        public void alloctor_init()
        {
            //var pooledbufAlloc = new PooledThreadLocalBytebufAlloctor(8);

            var alloc = PooledThreadLocalBytebufAlloctor.Value;
            alloc.Buffer(16);
        }
    }
}
