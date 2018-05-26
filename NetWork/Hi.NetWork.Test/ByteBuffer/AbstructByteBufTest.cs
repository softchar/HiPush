using Hi.NetWork.Buffer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Hi.NetWork.Test.ByteBuffer
{

    [TestClass]
    public class AbstructByteBufTest
    {
        

        [TestInitialize]
        public void Initialize()
        {
            
        }

        #region Base

        [TestMethod]
        public void bytebuf_base_init_test()
        {
            
            IByteBuf buf = new AbstructByteBuf(256);
            Assert.AreEqual(buf.Capacity, 256);
            Assert.AreEqual(buf.ReadIndex, 0);
            Assert.AreEqual(buf.WriteIndex, 0);
            Assert.AreEqual(buf.Readables(), 0);
            Assert.AreEqual(buf.Writeables(), 256);
        }

        [TestMethod]
        public void bytebuf_base_setoffset_test()
        {
            IByteBuf buf = new AbstructByteBuf(256);
            buf.SetOffset(1);
            Assert.AreEqual(buf.Offset, 1);
            Assert.AreEqual(buf.Capacity, 255);

            try
            {
                buf.SetOffset(-1);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }

            try
            {
                buf.SetOffset(256);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void bytebuf_base_clear_test()
        {
            IByteBuf buf = new AbstructByteBuf(4);

            buf.SetOffset(1);

            buf.WriteByte((byte)1).ReadByte();

            Assert.AreEqual(buf.Offset, 1);
            Assert.AreEqual(buf.ReadIndex, 2);
            Assert.AreEqual(buf.WriteIndex, 2);
            Assert.AreEqual(buf.Readables(), 0);
            Assert.AreEqual(buf.Writeables(), 2);
            Assert.AreEqual(buf.Capacity, 3);

            buf.Clear();
            Assert.AreEqual(buf.Offset, 1);
            Assert.AreEqual(buf.ReadIndex, 1);
            Assert.AreEqual(buf.WriteIndex, 1);
            Assert.AreEqual(buf.Readables(), 0);
            Assert.AreEqual(buf.Writeables(), 3);
            Assert.AreEqual(buf.Capacity, 3);
            Assert.AreNotEqual(buf.GetBytes(), AbstructByteBuf.Emtpy);

        }

        [TestMethod]
        public void bytebuf_base_return_test()
        {
            IByteBuf buf = new AbstructByteBuf(4);

            buf.SetOffset(1).WriteByte(1).ReadByte();

            Assert.AreEqual(buf.Offset, 1);
            Assert.AreEqual(buf.ReadIndex, 2);
            Assert.AreEqual(buf.WriteIndex, 2);
            Assert.AreEqual(buf.Readables(), 0);
            Assert.AreEqual(buf.Writeables(), 2);
            Assert.AreEqual(buf.Capacity, 3);

            buf.Return();

            Assert.AreEqual(buf.Offset, 0);
            Assert.AreEqual(buf.ReadIndex, 0);
            Assert.AreEqual(buf.WriteIndex, 0);
            Assert.AreEqual(buf.Readables(), 0);
            Assert.AreEqual(buf.Writeables(), 0);
            Assert.AreEqual(buf.Capacity, 0);
            Assert.AreEqual(buf.GetBytes(), AbstructByteBuf.Emtpy);

        }

        [TestMethod]
        public void bytebuf_base_blockcopy_test()
        {
            byte[] arr = { 0, 1, 2, 3, 4, 5, 6 };
            var buf1 = new AbstructByteBufFaker(256);

            /*
             * 新建一个容量为256字节的bytebuf,将arr全部写入buf之中,即offset:0,count:arr.Length,验证各个属性值
             */ 
            buf1.BlockCopyFaker(arr, 0, arr.Length);

            Assert.AreEqual(buf1.ReadIndex, 0);
            Assert.AreEqual(buf1.WriteIndex, 7);
            Assert.AreEqual(buf1.Readables(), 7);
            Assert.AreEqual(buf1.Writeables(), 249);

            /*
             * 2.调用BlockCopy,输入参数offset:-1,count:arr.Length,抛出ArgumentException
             */
            try
            {
                buf1.BlockCopyFaker(arr, -1, arr.Length);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            catch (Exception)
            {
                //这里是保证异常必须一定是IndexOutOfRangeException
                Assert.Fail();
            }


            /*
             * 3. 调用BlockCopy,输入参数offset:0,count:-1,抛出IndexOutOfRangeException
             */
            var buf3 = new AbstructByteBufFaker(4);
            try
            {
            	buf3.BlockCopyFaker(arr, 0, -1);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException) { }
            catch (System.Exception)
            {
                Assert.Fail();
            }


            /*
             * 4. 调用BlockCopy,输入参数offset:0,count:arr.Length+1,抛出ArgumentException
             */
            var buf4 = new AbstructByteBufFaker(4);
            try
            {
                buf4.BlockCopyFaker(arr, 0, arr.Length + 1);
                Assert.Fail();
            }
            catch (ArgumentException) { }
            catch (Exception)
            {
                Assert.Fail();
            }


            /*
             * 5. 调用BlockCopy,输入参数offset:3,count:5,抛出ArgumentException
             */
            var buf5 = new AbstructByteBufFaker(4);
            try
            {
                buf5.BlockCopyFaker(arr, 3, 5);
                Assert.Fail();
            }
            catch (ArgumentException) { }
            catch (Exception)
            {
                Assert.Fail();
            }


            /*
             * 6. 调用BlockCopy,输入arr=null,抛出ArgumentException
             */
            var buf6 = new AbstructByteBufFaker(4);
            try
            {
                buf6.BlockCopyFaker(null, 0, 0);
                Assert.Fail();
            }
            catch (ArgumentException) { }
            catch (Exception)
            {
                Assert.Fail();
            }


            /*
             * 7. 新建一个容量为4的bytebuf,将arr全部写入buf之中,即offset:0,count:arr.Length,
             * 此时的情况是bytebuf不足以接收arr的数据量,抛出IndexOutOfRangeException异常
             */
            var buf7 = new AbstructByteBufFaker(4);
            try
            {
                buf7.BlockCopyFaker(arr, 0, arr.Length);
                Assert.Fail();
            }
            catch (ArgumentException) { }
            catch (Exception)
            {
                Assert.Fail();
            }
           
        } 

        [TestMethod]
        public void bytebuf_base_init_setbytes_test()
        {
            IByteBuf buf = new AbstructByteBuf();
            byte[] arr = new byte[256];
            buf.SetBytes(arr);

            Assert.AreEqual(buf.Offset, 0);
            Assert.AreEqual(buf.ReadIndex, 0);
            Assert.AreEqual(buf.WriteIndex, 0);
            Assert.AreEqual(buf.Readables(), 0);
            Assert.AreEqual(buf.Writeables(), 256);

            buf.Clear();
        }

        [TestMethod]
        public void bytebuf_base_init_setbytes_offset_test()
        {
            IByteBuf buf = new AbstructByteBuf();
            byte[] arr = new byte[256];
            buf.SetBytes(arr, 10, 64);

            Assert.AreEqual(buf.Offset, 10);
            Assert.AreEqual(buf.ReadIndex, 10);
            Assert.AreEqual(buf.WriteIndex, 10);
            Assert.AreEqual(buf.Readables(), 0);
            Assert.AreEqual(buf.Writeables(), 64);

        }

        #endregion

        #region Read & Write

        [TestMethod]
        public void bytebuf_writeread_byte_test()
        {
            IByteBuf buf = new AbstructByteBuf(256);

            buf.SetOffset(1);
            buf.WriteByte((byte)1);

            Assert.AreEqual(buf.Offset, 1);
            Assert.AreEqual(buf.ReadIndex, 1);
            Assert.AreEqual(buf.WriteIndex, 2);
            Assert.AreEqual(buf.Readables(), 1);
            Assert.AreEqual(buf.Writeables(), 254);

            byte b1 = buf.ReadByte();
            Assert.AreEqual(b1, 1);

            Assert.AreEqual(buf.Offset, 1);
            Assert.AreEqual(buf.ReadIndex, 2);
            Assert.AreEqual(buf.WriteIndex, 2);
            Assert.AreEqual(buf.Readables(), 0);
            Assert.AreEqual(buf.Writeables(), 254);


        }

        [TestMethod]
        public void bytebuf_writeread_int_test()
        {
            IByteBuf buf = new AbstructByteBuf(256);

            buf.SetOffset(1);
            buf.Write(1);

            Assert.AreEqual(buf.Offset, 1);
            Assert.AreEqual(buf.ReadIndex, 1);
            Assert.AreEqual(buf.WriteIndex, 5);
            Assert.AreEqual(buf.Readables(), 4);
            Assert.AreEqual(buf.Writeables(), 251);

            int b = buf.ReadInt32();
            Assert.AreEqual(b, 1);

            Assert.AreEqual(buf.Offset, 1);
            Assert.AreEqual(buf.ReadIndex, 5);
            Assert.AreEqual(buf.WriteIndex, 5);
            Assert.AreEqual(buf.Readables(), 0);
            Assert.AreEqual(buf.Writeables(), 251);

        }

        [TestMethod]
        public void bytebuf_write_array_test()
        {
            IByteBuf buf = new AbstructByteBuf(256);
            byte[] arr = { 0, 1, 2, 3, 4 };

            buf.SetOffset(1);
            buf.Write(arr);

            Assert.AreEqual(buf.Offset, 1);
            Assert.AreEqual(buf.ReadIndex, 1);
            Assert.AreEqual(buf.WriteIndex, 6);
            Assert.AreEqual(buf.Readables(), 5);
            Assert.AreEqual(buf.Writeables(), 250);

        }

        [TestMethod]
        public void bytebuf_write_array_part_test()
        {
            IByteBuf buf = new AbstructByteBuf(256);
            byte[] arr = { 0, 1, 2, 3, 4, 5, 6 };

            buf.SetOffset(1);
            buf.Write(arr, 2, 5);
            

            Assert.AreEqual(buf.Offset, 1);
            Assert.AreEqual(buf.ReadIndex, 1);
            Assert.AreEqual(buf.WriteIndex, 6);
            Assert.AreEqual(buf.Readables(), 5);
            Assert.AreEqual(buf.Writeables(), 250);

        }

        [TestMethod]
        public void bytebuf_write_bytebuf_test()
        {
            var buf1 = new AbstructByteBuf(256);
            var buf2 = new AbstructByteBuf(7);
            for (byte i = 0; i < 7; i++)
            {
                buf2.WriteByte(i);
            }

            buf1.SetOffset(1);
            buf1.Write(buf2);

            Assert.AreEqual(buf1.Offset, 1);
            Assert.AreEqual(buf1.ReadIndex, 1);
            Assert.AreEqual(buf1.WriteIndex, 8);
            Assert.AreEqual(buf1.Readables(), 7);
            Assert.AreEqual(buf1.Writeables(), 242);
        }

        [TestMethod]
        public void bytebuf_write_bytebuf_offset_test()
        {
            var buf1 = new AbstructByteBuf(256);
            var buf2 = new AbstructByteBuf(9);
            for (byte i = 0; i < 9; i++)
            {
                buf2.WriteByte(i);
            }

            buf1.SetOffset(1);
            buf1.Write(buf2, 7);

            Assert.AreEqual(buf1.Offset, 1);
            Assert.AreEqual(buf1.ReadIndex, 1);
            Assert.AreEqual(buf1.WriteIndex, 8);
            Assert.AreEqual(buf1.Readables(), 7);
            Assert.AreEqual(buf1.Writeables(), 248);
        }

        #endregion

        class AbstructByteBufFaker : AbstructByteBuf
        {

            public AbstructByteBufFaker(int capacity)
                : base(capacity)
            {

            }

            public void BlockCopyFaker(byte[] array, int offset, int count)
            {
                BlockCopy(array, offset, count);
            }
        }
    }
}
