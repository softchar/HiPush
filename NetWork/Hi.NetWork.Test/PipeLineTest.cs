using Hi.NetWork.Socketing;
using Hi.NetWork.Socketing.ChannelPipeline;
using Hi.NetWork.Socketing.Channels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Test {

    [TestClass]
    public class PipeLineTest {

        [TestMethod]
        public void pipeline_addlast_test() {

            var pipeline = new DefaultChannelPipeline();

            var Context1 = new channelHandler1();
            var Context2 = new channelHandler2();
            //var Context3 = new ChannelHandler();
            //var Context4 = new ChannelHandler();
            //var Context5 = new ChannelHandler();

            pipeline.AddLast("",Context1);
            pipeline.AddLast("",Context2);
            //pipeline.AddLast(Context3);
            //pipeline.AddLast(Context4);
            //pipeline.AddLast(Context5);

            Assert.AreEqual(pipeline.Head, Context1);
            Assert.AreEqual(pipeline.Head.Next, Context2);
            //Assert.AreEqual(pipeline.Head.Next().Next(), Context3);
            //Assert.AreEqual(pipeline.Head.Next().Next().Next(), Context4);
            //Assert.AreEqual(pipeline.Head.Next().Next().Next().Next(), Context5);

            //pipeline.OnConnected();
            //pipeline.OnReceived(new ChannelMessage());

            Assert.IsTrue(true);

        }

        class channelHandler1 : ChannelHandler {

            //public override bool OnConnected(IChannel channel) {

            //    Console.WriteLine(@"channelHandler1.OnConnected");

            //    return true;

            //}

            //public override bool OnReceived(IChannel channel, ChannelMessage message) {

            //    Console.WriteLine(@"channelHandler1.OnReceived");

            //    return true;

            //}
        }

        class channelHandler2 : ChannelHandler {

            //public override bool OnConnected(IChannel channel) {

            //    Console.WriteLine(@"channelHandler2.OnConnected");

            //    return true;

            //}

            //public override bool OnReceived(IChannel channel, ChannelMessage message) {

            //    Console.WriteLine(@"channelHandler2.OnReceived");

            //    return true;

            //}

        }

    }
}
