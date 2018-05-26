using Hi.NetWork.Buffer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Hi.NetWork.Test.ByteBuffer
{
    /************************************************************************/
    /*  
     *  功能需求
     *  
     *  1：初始化容器
     *  2：读写测试
     *  3：清空/重置ByteBuf
     *  4：跳过读写索引
     *  5：修改读写索引
     *  6：分片
     *  
     *  
     *  
     *  测试需求
     *  
     *  1：初始化FixedLengthByteBuf，readIndex=0，writeIndex=0
     *  
     *  2：读写Int32
     *  
     *     写入int类型的值65535，readIndex=0，writeIndex=4
     *     写入int类型的值1，readIndex=0，writeIndex=8
     *     写入int类型的值65535，readIndex=0，writeIndex=12
     *     
     *     读取一个int类型的值value，readIndex=4，writeIndex=12，value=65535
     *     继续读取一个int类型的值value，readIndex=8，writeIndex=12，value=1
     *     再读取一个int类型的值value，readIndex=12，writeIndex=12，value=65535
     *                       
                                                                            */
    /************************************************************************/

    [TestClass]
    public class FixedLengthByteBufTest
    {

        FixedLengthByteBuf fixedByteBuf;

        [TestInitialize]
        public void Init()
        {
            fixedByteBuf = new FixedLengthByteBuf(4096);
        }

        /// <summary>
        /// 1：初始化FixedLengthByteBuf，readIndex = 0，writeIndex = 0
        /// </summary>
        [TestMethod]
        public void InitTest()
        {
            Assert.AreEqual(fixedByteBuf.ReadIndex, 0);
            Assert.AreEqual(fixedByteBuf.WriteIndex, fixedByteBuf.ReadIndex);

        }

        /// <summary>
        /// 2：读写Int32
        ///     写入int类型的值65535，readIndex=0，writeIndex=4
        ///     写入int类型的值1，readIndex=0，writeIndex=8
        ///     写入int类型的值65535，readIndex=0，writeIndex=12
        ///     读取一个int类型的值value，readIndex=4，writeIndex=12，value=65535
        ///     继续读取一个int类型的值value，readIndex=8，writeIndex=12，value=1
        ///     再读取一个int类型的值value，readIndex=12，writeIndex=12，value=65535
        /// </summary>
        [TestMethod]
        public void ReadAndWriteInt32Test()
        {
            int value = 65535;
            int one = 1;
            int Int32Size = sizeof(Int32);

            fixedByteBuf.Write(value);
            Assert.AreEqual(fixedByteBuf.WriteIndex, Int32Size);

            fixedByteBuf.Write(one);
            Assert.AreEqual(fixedByteBuf.WriteIndex, Int32Size * 2);

            fixedByteBuf.Write(value);
            Assert.AreEqual(fixedByteBuf.WriteIndex, Int32Size * 3);

            Assert.AreEqual(fixedByteBuf.ReadIndex, 0);

            int value1 = fixedByteBuf.ReadInt32();
            Assert.AreEqual(fixedByteBuf.ReadIndex, Int32Size);
            Assert.AreEqual(value1, value);

            int value2 = fixedByteBuf.ReadInt32();
            Assert.AreEqual(fixedByteBuf.ReadIndex, Int32Size * 2);
            Assert.AreEqual(value2, one);

            int value3 = fixedByteBuf.ReadInt32();
            Assert.AreEqual(fixedByteBuf.ReadIndex, Int32Size * 3);
            Assert.AreEqual(value3, value);

            Assert.AreEqual(fixedByteBuf.WriteIndex, Int32Size * 3);

        }

        /// <summary>
        /// 读取/写入跳过，readIndex/writeIndex跳过指定的长度开始读取
        /// </summary>
        [TestMethod]
        public void ReadAndWriteSkipTest()
        {
            int skipNum = 4;

            fixedByteBuf.WriteSkip(skipNum);
            fixedByteBuf.ReadSkip(skipNum);

            Assert.AreEqual(fixedByteBuf.WriteIndex, skipNum);
            Assert.AreEqual(fixedByteBuf.ReadIndex, skipNum);
            
        }

        /// <summary>
        /// 设置/重置ReadIndex和WriteIndex测试
        /// 设置readIndex，输入index=4，readIndex=4
        /// 设置writeIndex，输入index=4，writeIndex=4
        /// 重置readIndex，readIndex=0
        /// 重置writeIndex，writeIndex=0
        /// </summary>
        [TestMethod]
        public void SetIndexTest()
        {
            int index = 4;

            Assert.AreEqual(fixedByteBuf.ReadIndex, 0);

            fixedByteBuf.SetWriteIndex(index);
            fixedByteBuf.SetReadIndex(index);
            

            Assert.AreEqual(fixedByteBuf.ReadIndex, index);
            Assert.AreEqual(fixedByteBuf.WriteIndex, index);

            fixedByteBuf.ResetReadIndex();
            fixedByteBuf.ResetWriteIndex();

            Assert.AreEqual(fixedByteBuf.ReadIndex, 0);
            Assert.AreEqual(fixedByteBuf.WriteIndex, 0);
        }

        /// <summary>
        /// 读写越界，
        /// readIndex不能大于writeIndex，
        /// writeIndex不能大于maxIndex
        /// readIndex和writeIndex都不能大于maxIndex
        /// 
        /// 初始化FixedLengthByteBuf，capacity=3，readIndex=0，writeIndex=0
        /// 1.写入Int类型的值1，捕获IndexOutOfRangeException异常
        /// 2.读取Int类型的值，捕获IndexOutOfRangeException异常
        /// 3.设置readIndex=4，捕获IndexOutOfRangeException异常
        /// 4.设置writeIndex=4，捕获IndexOutOfRangeException异常
        /// 5.设置writeIndex=2，再设置readIndex=3，捕获IndexOutOfRangeException异常
        /// 6.设置readIndex=2，再设置writeIndex=1，捕获IndexOutOfRangeException异常
        /// </summary>
        [TestMethod]
        public void ReadOrWriteOutOfIndex()
        {
            var fixedLengthBuf = new FixedLengthByteBuf(3);

            //测试1.写入Int类型的值1，捕获IndexOutOfRangeException异常
            IndexOutOfRangeExceptionAction(()=> 
            {
                fixedLengthBuf.Write(1);
            });
            fixedLengthBuf.Clear();

            //测试2.读取Int类型的值，捕获IndexOutOfRangeException异常
            IndexOutOfRangeExceptionAction(() => 
            {
                fixedLengthBuf.ReadInt32();
            });
            fixedLengthBuf.Clear();

            //测试3.设置readIndex=4，捕获IndexOutOfRangeException异常
            IndexOutOfRangeExceptionAction(() => 
            {
                fixedLengthBuf.SetReadIndex(4);
            });
            fixedLengthBuf.Clear();

            //测试4.设置writeIndex=4，捕获IndexOutOfRangeException异常
            IndexOutOfRangeExceptionAction(() => 
            {
                fixedLengthBuf.SetWriteIndex(4);
            });
            fixedLengthBuf.Clear();

            //测试5.设置writeIndex=2，再设置readIndex=3，捕获IndexOutOfRangeException异常
            fixedLengthBuf.SetWriteIndex(2);
            IndexOutOfRangeExceptionAction(() => 
            {
                fixedLengthBuf.SetReadIndex(3);
            });
            fixedLengthBuf.Clear();

            //6.设置readIndex=2，再设置writeIndex=1，捕获IndexOutOfRangeException异常
            fixedLengthBuf.SetWriteIndex(2);
            fixedLengthBuf.SetReadIndex(2);
            IndexOutOfRangeExceptionAction(() => 
            {
                fixedLengthBuf.SetWriteIndex(1);
            });
            fixedLengthBuf.Clear();

        }

        /// <summary>
        /// 接收到IndexOutOfRangeException表示成功，否则失败
        /// </summary>
        /// <param name="action"></param>
        private void IndexOutOfRangeExceptionAction(Action action)
        {
            try
            {
                action();
                Assert.Fail();
            }
            catch (IndexOutOfRangeException)
            {
                Assert.IsTrue(true);
            }
        }

    }
}
