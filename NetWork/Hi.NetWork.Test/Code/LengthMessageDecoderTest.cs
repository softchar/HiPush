using Hi.NetWork.Buffer;
using Hi.NetWork.Code;
using Hi.NetWork.Socketing.ChannelPipeline;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.NetWork.Socketing.Channels;
using System.Net;

namespace Hi.NetWork.Test.Code
{
    [TestClass]
    public class LengthMessageDecoderTest
    {
        FakerLengthMessageDecoder decoder;
        FakerChannelHandlerContext ctx;

        [TestInitialize]
        public void Init()
        {
            decoder = new FakerLengthMessageDecoder();
            ctx = new FakerChannelHandlerContext(new FakerByteBufAllocator());
        }

        [TestMethod]
        public void onemessage()
        {
            string AText = "AAAAAA";
            var bytes = new byte[10] { 06, 00, 00, 00, 65, 65, 65, 65, 65, 65 };

            var buf = AbstructByteBuf.Small();

            buf.Write(bytes);

            var output = new List<object>();

            decoder.Decoder(ctx, buf, output);

            Assert.IsTrue(output != null && output.Count == 1);
            Assert.AreEqual(ctx.IncompleteMessage, null);

            var result = output[0] as IByteBuf;
            var resultText = System.Text.Encoding.UTF8.GetString(result.GetBytes(), result.ReadIndex, result.Readables());

            Assert.AreEqual(AText, resultText);
            
        }

        /// <summary>
        /// 一个完整包+一个半包（半包包含长度）
        /// </summary>
        [TestMethod]
        public void one_and_incomplete()
        {
            string AText = "AAAAAA";
            var bytes = new byte[15] { 06, 00, 00, 00, 65, 65, 65, 65, 65, 65, 06, 00, 00, 00, 66 };
            var incomplete = new byte[1] { 66 };
            var buf = AbstructByteBuf.Small();
            buf.Write(bytes);

            var output = new List<object>();

            decoder.Decoder(ctx, buf, output);

            Assert.IsTrue(output != null && output.Count == 1);
            Assert.IsTrue(ctx.IncompleteMessage != null);
            Assert.AreEqual(ctx.IncompleteHeader, 6);

            for (int i = 0; i < ctx.IncompleteMessage.Readables(); i++)
            {
                int readindex = ctx.IncompleteMessage.ReadIndex;
                Assert.AreEqual(ctx.IncompleteMessage.GetBytes()[readindex + i], incomplete[i]);
            }

            var result = output[0] as IByteBuf;
            var resultText = System.Text.Encoding.UTF8.GetString(result.GetBytes(), result.ReadIndex, result.Readables());

            Assert.AreEqual(AText, resultText);
        }

        /// <summary>
        /// 一个完整包+一个半包（半包不包含长度）
        /// </summary>
        /// <remarks>
        /// 当半包数据长度大于4个字节（一个int类型的字节长度）时，长度已经读取出来了，保存在ctx.IncompleteLength，所以ctx.IncompleteMessage不会包含长度帧的数据
        /// 当半包数据长度小于4个字节时，长度因为字节不够4，所以不能确定长度是多少，此时ctx.IncompleteMessage包含长度帧的数据
        /// 当读取半包数据时，如果ctx.IncompleteLength==0,那么需要先读取长度帧，如果ctx.IncompleteLength>0，直接读取数据
        /// </remarks>
        [TestMethod]
        public void one_and_incomplete_nolengthframe()
        {
            string AText = "AAAAAA";
            var bytes = new byte[13] { 06, 00, 00, 00, 65, 65, 65, 65, 65, 65, 06, 00, 00 };
            var incomplete = new byte[3] { 06, 00, 00 };
            var buf = AbstructByteBuf.Small();
            buf.Write(bytes);

            var output = new List<object>();

            decoder.Decoder(ctx, buf, output);

            Assert.IsTrue(output != null && output.Count == 1);
            Assert.IsTrue(ctx.IncompleteMessage == null);
            Assert.IsTrue(ctx.IncompleteLength == 3);

            var result = output[0] as IByteBuf;
            var resultText = System.Text.Encoding.UTF8.GetString(result.GetBytes(), result.ReadIndex, result.Readables());

            Assert.AreEqual(AText, resultText);
        }

        /// <summary>
        /// 一个半包 + 一个半包 = 一个整包
        /// </summary>
        [TestMethod]
        public void incomplete()
        {
            string AText = "BBBBBB";

            /*
             
            原始数据第一个包 { 06, 00, 00, 00, 66 }
            第二个包 {66, 66, 66, 66, 66 }
            半包数据 { 66 }
            包长 6
            包长已读：4
             
             */

            var bytes1 = new byte[5] { 06, 00, 00, 00, 66 };
            var bytes2 = new byte[5] { 66, 66, 66, 66, 66 };

            var output = new List<object>();

            //数据包
            var buf = AbstructByteBuf.Small().Write(bytes2);

            //虚构一个半包
            ctx.IncompleteMessage = AbstructByteBuf.Small().WriteByte(66);
            ctx.IncompleteHeader = 06;
            ctx.IncompleteLength = 4;

            decoder.Decoder(ctx, buf, output);

            Assert.AreEqual(output.Count, 1);
            var outbuf = (IByteBuf)output[0];
            var outstring = System.Text.Encoding.UTF8.GetString(outbuf.GetBytes(), outbuf.ReadIndex, outbuf.Readables());
            Assert.AreEqual(outstring, AText);

        }

        /// <summary>
        /// 一个半包 + 一个半包 = 1个半包
        /// </summary>
        public void incomplete2()
        {

        }

        public class FakerLengthMessageDecoder : LengthMessageDecoder
        {

            public void Decoder(IChannelHandlerContext ctx, IByteBuf buf, List<object> output)
            {
                this.Decode(ctx, buf, output);
            }

        }

        public class FakerChannelHandlerContext : IChannelHandlerContext
        {
            IByteBufAllocator alloc;
            public FakerChannelHandlerContext(IByteBufAllocator alloc)
            {
                this.alloc = alloc;
            }
            public IByteBufAllocator Alloc
            {
                get
                {
                    return alloc;
                }
            }

            public IChannel Channel
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IChannelHandler Handler
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            private IByteBuf incompleteMessage;
            public IByteBuf IncompleteMessage
            {
                get
                {
                    return incompleteMessage;
                }

                set
                {
                    incompleteMessage = value;
                }
            }

            public LifeCycleFlag LifeCycleFlag
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IChannelHandlerContext Next
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public IChannelHandlerContext Prev
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            private int inCompleteLength;
            public int IncompleteLength
            {
                get
                {
                    return inCompleteLength;
                }

                set
                {
                    inCompleteLength = value;
                }
            }

            private int incompleteHeader;
            public int IncompleteHeader
            {
                get
                {
                    return incompleteHeader;
                }

                set
                {
                    incompleteHeader = value;
                }
            }

            public void fireChannelActive()
            {
                throw new NotImplementedException();
            }

            public void fireChannelClose()
            {
                throw new NotImplementedException();
            }

            public void fireChannelException()
            {
                throw new NotImplementedException();
            }

            public void fireChannelFinally()
            {
                throw new NotImplementedException();
            }

            public void fireChannelRead(object messasge)
            {
                throw new NotImplementedException();
            }

            public void fireChannelRegister()
            {
                throw new NotImplementedException();
            }

            public void fireChannelWrite(object message)
            {
                throw new NotImplementedException();
            }

            public bool IsLifeCycle(LifeCycleFlag lifeCycleFlag)
            {
                throw new NotImplementedException();
            }

            public Task WriteAsync(object msg)
            {
                throw new NotImplementedException();
            }

            public Task BindAsync(EndPoint address)
            {
                throw new NotImplementedException();
            }

            public Task ConnectAsync(EndPoint remote)
            {
                throw new NotImplementedException();
            }
        }

        public class FakerByteBufAllocator : IByteBufAllocator
        {
            public IByteBuf Buffer(int size)
            {
                return new FixedLengthByteBuf(size);
            }

           
        }
    }
}
