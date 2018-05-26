using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.NetWork.Buffer;
using System.Diagnostics;

namespace Hi.NetWork.Test.ByteBuffer
{
    /************************************************************************/
    /* 
     * 本地线程内存池测试
     * 
     * 功能：
     * 初始化一定数量(m)的ByteBuf队列；
     * 每次将队列的第一个ByteBuf弹出以供使用
     * 回收ByteBuf，将需要回收的ByteBuf插入到队列的末端
     * 当m个ByteBuf全部使用完之后，自动将队列补充n个ByteBuf，ByteBuf的总数量不能操作总数量t
     * 
     * 测试需求：
     * 1、初始化内存池，bufferCounter=2，maxBufferCounter=5，increment = 2
     * 2、获取2个ByteBuf，Count=0   
     * 3、再获取1个ByteBuf，内存池因为空了，会自动扩展2个ByteBuf，Count=2   
     *                                                                      */
    /************************************************************************/

    [TestClass]
    public class ThreadLocalPooledByteBufTest
    {

        IBytebufPool pooledByteBuf;

        //Buffer数量最大为20个
        int maxBufferCounter = 20;
        //默认数量为5
        int bufferCounter = 5;
        //增量为5
        int increment = 5;

        [TestInitialize]
        public void init()
        {

            ////1、初始化内存池，bufferCounter = 5，maxBufferCounter = 20
            //pooledByteBuf = new ThreadLocalPooledByteBuf(
            //    maxBufferCounter,
            //    bufferCounter,
            //    increment,
            //    () => { return new FixedLengthByteBuf(); }
            //).Init();
        }

        /// <summary>
        /// 初始化测试
        /// </summary>
        [TestMethod]
        public void InitTest()
        {

            Assert.AreEqual(pooledByteBuf.MaxBufferCounter, maxBufferCounter);
            Assert.AreEqual(pooledByteBuf.Available, bufferCounter);

            //2、获取5个ByteBuf，Count = 0
            for (int i = 0; i < 5; i++)
            {
                pooledByteBuf.Get();
            }

            Assert.AreEqual(pooledByteBuf.Available, 0);

            //3、再获取5个ByteBuf，内存池因为空了，会自动扩展5个ByteBuf，Available=4，References=6，Count=10(Available+References)
            var byteBuf3 = pooledByteBuf.Get();
            Assert.AreEqual(pooledByteBuf.Available, 4);
            Assert.AreEqual(pooledByteBuf.References, 6);
            Assert.AreEqual(pooledByteBuf.Count, 10);


        }

        /// <summary>
        /// 释放测试
        /// </summary>
        [TestMethod]
        public void ReleaseTest()
        {
            var buf = pooledByteBuf.Get();

            //打乱ReadIndex和WriteIndex
            buf.Write(1);
            buf.ReadInt32();

            Assert.AreEqual(pooledByteBuf.Count, bufferCounter);
            Assert.AreEqual(pooledByteBuf.Available, bufferCounter-1);
            Assert.AreEqual(pooledByteBuf.References, 1);

            var isSuccess = pooledByteBuf.Return(buf);

            Assert.IsTrue(isSuccess);
            Assert.AreEqual(pooledByteBuf.Count, bufferCounter);
            Assert.AreEqual(pooledByteBuf.Available, bufferCounter);
            Assert.AreEqual(pooledByteBuf.References, 0);

            //在释放的同时，需要清空bytebuf的数据
            Assert.AreEqual(buf.ReadIndex, 0);
            Assert.AreEqual(buf.WriteIndex, 0);

            var isSuccess2 = pooledByteBuf.Return(new FixedLengthByteBuf());
            Assert.IsFalse(isSuccess2);
            

        }

        /// <summary>
        /// 扩展测试
        /// </summary>
        [TestMethod]
        public void ExpensionTest()
        {
            for (int i = 0; i < maxBufferCounter; i++)
            {
                //在缓冲区消耗完之前每执行一次Get之前，都需要进行一次是否可用的判断。
                //确保在缓冲区消耗完之前IsAvailable=true
                Assert.IsTrue(pooledByteBuf.IsAvailable);
                pooledByteBuf.Get();
            }

            var empty = pooledByteBuf.Get();

            Assert.AreEqual(empty, null);
            Assert.IsTrue(pooledByteBuf.IsAvailable);
           
        }

        [TestMethod]
        public void PerformanceTest()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < 10000; i++)
            {
                var buf = pooledByteBuf.Get();
                buf.Return();
            }
            watch.Stop();
            Console.WriteLine($"运行时间：{watch.ElapsedMilliseconds}");


            Stopwatch watch2 = new Stopwatch();
            watch2.Start();
            for (int i = 0; i < 10000; i++)
            {
                var buf = new FixedLengthByteBuf();
                buf.Return();
            }
            watch2.Stop();
            Console.WriteLine($"运行时间：{watch2.ElapsedMilliseconds}");
        }
    }
}
