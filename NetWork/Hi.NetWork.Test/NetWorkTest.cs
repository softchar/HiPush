using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Hi.NetWork.Protocols;

namespace Hi.NetWork.Test {

    [TestClass]
    public class NetWorkTest {

        [TestMethod]
        public void TalkWithServer() {
            IPAddress.Parse("127.0.0.1");
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 23456));

            var msg = "Hello,Word!";
            var body = new MessageBody() { Data = msg, TagId = "10086", Type = 100 };
            string bodyJson = Newtonsoft.Json.JsonConvert.SerializeObject(body);

            var pack = MessageResolver.BuildPack(bodyJson);
            int result = socket.Send(pack.Data);
            byte[] receiveData = new byte[1024];
            socket.Receive(receiveData);

            string str = Encoding.UTF8.GetString(receiveData);
        } 
    }
}
