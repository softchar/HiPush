using Hi.NetWork.Protocols;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Test {

    /************************************************************************/
    /* 1:输入A，输出byte[]{1,0,0,0,65}
     * 2:输入AB，输出byte[]{2,0,0,0,65,66}
    /************************************************************************/
    [TestClass]
    public class MessagePacketTest {

        /// <summary>
        /// 输入A，输出byte[]{1,0,0,0,65}
        /// </summary>
        [TestMethod]
        public void singleChar() {
            string str = "A";
            byte[] bytes = new byte[5] { 1, 0, 0, 0, 65 };

            var pack = MessageResolver.BuildPack(str);

            Assert.AreEqual(pack.Length, bytes.Length);

            for (int i = 0; i < pack.Length; i++) {
                Assert.AreEqual(pack.Data[i], bytes[i]);
            }
        }

        /// <summary>
        /// 输入AB，输出byte[]{2,0,0,0,65,66}
        /// </summary>
        [TestMethod]
        public void multipleChar() {
            string str = "AB";
            byte[] bytes = new byte[6] { 2, 0, 0, 0, 65, 66 };

            var pack = MessageResolver.BuildPack(str);

            Assert.AreEqual(pack.Length, bytes.Length);

            for (int i = 0; i < pack.Length; i++) {
                Assert.AreEqual(pack.Data[i], bytes[i]);
            }
        }
    }
}
