using Hi.NetWork.Buffer;
using Hi.NetWork.Eventloops;
using Hi.NetWork.Socketing.Channels;
using Hi.NetWork.Socketing.Channels.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Test.Channel
{
    /*
     * 需求列表
     * 1，添加一个bytebuf
     * 2，获取一个bytebuf
     * 3，当Buffer中的已经满了的时候，IsWritable=false，继续写会抛出异常OutOfBufferException
     * 
     * 测试列表
     * 添加一个PendingMessage，counter=1
     * 获取一个PendingMessage，counter=0
     * 设置MaxCount=2，添加两个PendingMessage,IsWriteable=false，再添加一个PendingMessage，
     *     抛出异常OutOfBufferException
     */
    [TestClass]
    public class OutBoundBufferTest
    {
        [TestMethod]
        public void outboundbuffer_add_test()
        {
            var buf = new FixedLengthByteBuf();
            var buffer = new OutBoundBuffer();

            buffer.AddLast(new PendingMessage(buf, new TaskCompletionSource()));

            Assert.AreEqual(buffer.Count, 1);

            var msg = buffer.Get();

            Assert.AreEqual(buffer.Count, 0);

        }

        [TestMethod]
        public void outboundbuffet_outof_test()
        {
            var outBoundBuffer = new OutBoundBuffer(2);

            var buf1 = new FixedLengthByteBuf();
            var buf2 = new FixedLengthByteBuf();
            var promise1 = new TaskCompletionSource();
            var promise2 = new TaskCompletionSource();

            outBoundBuffer.Add(new PendingMessage(buf1, promise1));
            Assert.AreEqual(outBoundBuffer.Count, 1);
            Assert.AreEqual(outBoundBuffer.IsWritable, true);
            outBoundBuffer.Add(new PendingMessage(buf2, promise2));
            Assert.AreEqual(outBoundBuffer.Count, 2);
            Assert.AreEqual(outBoundBuffer.IsWritable, false);

            try
            {
            	outBoundBuffer.Add(new PendingMessage(buf2, promise2));
                Assert.Fail();
            }
            catch (OutOfBufferException)
            {
                Assert.IsTrue(true);
            }
        }
    }
}
