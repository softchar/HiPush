using Hi.NetWork.Protocols;
using Hi.NetWork.Socketing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Test {

    /// <summary>
    /// 协议测试类
    ///     协议最主要的作用就是将数据解析为感兴趣的对象
    /// </summary>
    [TestClass]
    public class ProtocolTest {

        [TestMethod]
        public void protocol_test() {

            var protocol = new DefaultProtocol();

            var evt = new HiEvent();

            var json = JsonConvert.SerializeObject(evt);

            var bytes = Encoding.UTF8.GetBytes(json);

            var hievent = protocol.Serialize(bytes);

            Assert.AreEqual(evt.TagId.ToString(), hievent.TagId.ToString());
            Assert.AreEqual(evt.Type, hievent.Type);
            Assert.AreEqual(evt.TimeStamp, hievent.TimeStamp);
            Assert.AreEqual(evt.Data, hievent.Data);

        } 

        

    }
}
