using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.NetWork.Socketing;
using System.IO;
using System.IO;
using Hi.NetWork.Buffer;

namespace Hi.NetWork.Test {

    [TestClass]
    public class ChannelTest {

        //////////////////////////////////////////////////////////////////////////
        /*
         *  1,设置sendingBuffSize=4,输入[1,2,3,4],输出:true, 结果集:[1,2,3,4]
         *  2,设置sendingBuffSize=5,输入[[1,2,3,4],[5,6,7,8]],输出:true, 结果集:[1,2,3,4,5], MemoryStream:[6,7,8]
         */
        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 1,设置sendingBuffSize=4,输入[1,2,3,4],输出:true, 结果集:[1,2,3,4]
        /// </summary>
        [TestMethod]
        public void sending_arraysegment_1array_test() {

            var bytes = new byte[] { 1, 2, 3, 4 };

            var _channel = new TempChannel();

            _channel.SendingBufferSize = 4;

            _channel.appendToSendingQueue(new ArraySegment<byte>(bytes));

            ArraySegment<byte> _segment;

            bool _result = _channel.GetSendingSegment(out _segment);

            Assert.IsTrue(_result);
            Assert.IsTrue(_segment != null);
            Assert.AreEqual(bytes[0], _segment.Array[0]);
            Assert.AreEqual(bytes[1], _segment.Array[1]);
            Assert.AreEqual(bytes[2], _segment.Array[2]);
            Assert.AreEqual(bytes[3], _segment.Array[3]);

        }

        [TestMethod]
        /// <summary>
        /// 2,设置sendingBuffSize=5,输入[[1,2,3,4],[5,6,7,8]],输出:true, 结果集:[1,2,3,4,5], LastSendingSegment:[6,7,8]
        /// </summary>
        public void sending_arraysegment_2array_test() {

            var bytes1 = new byte[] { 1, 2, 3, 4 };
            var bytes2 = new byte[] { 5, 6, 7, 8 };

            var _channel = new TempChannel();

            _channel.SendingBufferSize = 5;

            _channel.appendToSendingQueue(new ArraySegment<byte>(bytes1));
            _channel.appendToSendingQueue(new ArraySegment<byte>(bytes2));

            ArraySegment<byte> _segment;

            bool _result = _channel.GetSendingSegment(out _segment);

            Assert.IsTrue(_result);
            Assert.IsTrue(_segment != null);
            Assert.AreEqual(bytes1[0], _segment.Array[0]);
            Assert.AreEqual(bytes1[1], _segment.Array[1]);
            Assert.AreEqual(bytes1[2], _segment.Array[2]);
            Assert.AreEqual(bytes1[3], _segment.Array[3]);
            Assert.AreEqual(bytes2[0], _segment.Array[4]);

            var _lastSendingSegment = _channel.GetLastSendingSegment();
            Assert.IsTrue(_lastSendingSegment.Array != null);

            Assert.IsTrue(_lastSendingSegment.Count == 3);
            Assert.AreEqual(_lastSendingSegment.Array[1], bytes2[1]);
            Assert.AreEqual(_lastSendingSegment.Array[2], bytes2[2]);
            Assert.AreEqual(_lastSendingSegment.Array[3], bytes2[3]);
            

        }

        [TestMethod]
        /// <summary>
        /// 3,设置sendingBufferSize=5,输入[[1,2,3,4],[5,6,7,8]],[9,10,11,12](3个数组分两次发送),输出true,结果集:2个segment, LastSendingSegment:[11,12]
        /// </summary>
        public void sending_arraysegment_2_3array_test() {
            var bytes1 = new byte[] { 1, 2, 3, 4 };
            var bytes2 = new byte[] { 5, 6, 7, 8 };
            var bytes3 = new byte[] { 9, 10, 11, 12 };

            var _channel = new TempChannel();

            _channel.SendingBufferSize = 5;

            _channel.appendToSendingQueue(new ArraySegment<byte>(bytes1));
            _channel.appendToSendingQueue(new ArraySegment<byte>(bytes2));

            ArraySegment<byte> _segment1;

            bool _result = _channel.GetSendingSegment(out _segment1);

            Assert.IsTrue(_result);
            Assert.IsTrue(_segment1.Array != null);
            Assert.AreEqual(bytes1[0], _segment1.Array[0]);
            Assert.AreEqual(bytes1[1], _segment1.Array[1]);
            Assert.AreEqual(bytes1[2], _segment1.Array[2]);
            Assert.AreEqual(bytes1[3], _segment1.Array[3]);
            Assert.AreEqual(bytes2[0], _segment1.Array[4]);

            var _lastSendingSegment = _channel.GetLastSendingSegment();
            Assert.IsTrue(_lastSendingSegment.Array != null);
            Assert.IsTrue(_lastSendingSegment.Count == 3);
            Assert.AreEqual(_lastSendingSegment.Array[1], bytes2[1]);
            Assert.AreEqual(_lastSendingSegment.Array[2], bytes2[2]);
            Assert.AreEqual(_lastSendingSegment.Array[3], bytes2[3]);

            _channel.appendToSendingQueue(new ArraySegment<byte>(bytes3));

            ArraySegment<byte> _segment2;

            bool _result2 = _channel.GetSendingSegment(out _segment2);

            Assert.IsTrue(_result2);
            Assert.IsTrue(_segment2.Array != null);
            Assert.AreEqual(bytes2[1], _segment2.Array[5]);
            Assert.AreEqual(bytes2[2], _segment2.Array[6]);
            Assert.AreEqual(bytes2[3], _segment2.Array[7]);
            Assert.AreEqual(bytes3[0], _segment2.Array[8]);
            Assert.AreEqual(bytes3[1], _segment2.Array[9]);

            var _lastSendingSegment2 = _channel.GetLastSendingSegment();
            Assert.IsTrue(_lastSendingSegment2.Array != null);
            Assert.IsTrue(_lastSendingSegment2.Count == 2);
            Assert.AreEqual(_lastSendingSegment2.Array[2], bytes3[2]);
            Assert.AreEqual(_lastSendingSegment2.Array[3], bytes3[3]);


        }

        [TestMethod]
        /// <summary>
        /// 4,设置sendingBufferSize=5,输入[[1, 2, 3, 4], [5, 6, 7, 8]], [[9, 10, 11, 12], [13, 14, 15, 16]]
        /// </summary>
        public void sending_arraysegment_mularray_test() {
            var bytes1 = new byte[] { 1, 2, 3, 4 };
            var bytes2 = new byte[] { 5, 6, 7, 8 };
            var bytes3 = new byte[] { 9, 10, 11, 12 };
            var bytes4 = new byte[] { 13, 14, 15, 16 };

            var _channel = new TempChannel();

            _channel.SendingBufferSize = 5;

            _channel.appendToSendingQueue(new ArraySegment<byte>(bytes1));
            _channel.appendToSendingQueue(new ArraySegment<byte>(bytes2));

            ArraySegment<byte> _segment1;

            bool _result = _channel.GetSendingSegment(out _segment1);

            Assert.IsTrue(_result);
            Assert.IsTrue(_segment1.Array != null);
            Assert.AreEqual(bytes1[0], _segment1.Array[0]);
            Assert.AreEqual(bytes1[1], _segment1.Array[1]);
            Assert.AreEqual(bytes1[2], _segment1.Array[2]);
            Assert.AreEqual(bytes1[3], _segment1.Array[3]);
            Assert.AreEqual(bytes2[0], _segment1.Array[4]);

            var _lastSendingSegment = _channel.GetLastSendingSegment();
            Assert.IsTrue(_lastSendingSegment.Array != null);
            Assert.IsTrue(_lastSendingSegment.Count == 3);
            Assert.AreEqual(_lastSendingSegment.Array[1], bytes2[1]);
            Assert.AreEqual(_lastSendingSegment.Array[2], bytes2[2]);
            Assert.AreEqual(_lastSendingSegment.Array[3], bytes2[3]);

            _channel.appendToSendingQueue(new ArraySegment<byte>(bytes3));
            _channel.appendToSendingQueue(new ArraySegment<byte>(bytes4));

            ArraySegment<byte> _segment2;

            bool _result2 = _channel.GetSendingSegment(out _segment2);

            Assert.IsTrue(_result2);
            Assert.IsTrue(_segment2.Array != null);
            Assert.AreEqual(bytes2[1], _segment2.Array[5]);
            Assert.AreEqual(bytes2[2], _segment2.Array[6]);
            Assert.AreEqual(bytes2[3], _segment2.Array[7]);
            Assert.AreEqual(bytes3[0], _segment2.Array[8]);
            Assert.AreEqual(bytes3[1], _segment2.Array[9]);

            var _lastSendingSegment2 = _channel.GetLastSendingSegment();
            Assert.IsTrue(_lastSendingSegment2.Array != null);
            Assert.IsTrue(_lastSendingSegment2.Count == 2);
            Assert.AreEqual(_lastSendingSegment2.Array[_lastSendingSegment2.Offset], bytes3[2]);
            Assert.AreEqual(_lastSendingSegment2.Array[_lastSendingSegment2.Offset + 1], bytes3[3]);


            ArraySegment<byte> _segment3;

            bool _result3 = _channel.GetSendingSegment(out _segment3);

            Assert.AreEqual(bytes3[2], _segment3.Array[10]);
            Assert.AreEqual(bytes3[3], _segment3.Array[11]);
            Assert.AreEqual(bytes4[0], _segment3.Array[12]);
            Assert.AreEqual(bytes4[1], _segment3.Array[13]);
            Assert.AreEqual(bytes4[2], _segment3.Array[14]);

            var _lastSendingSegment3 = _channel.GetLastSendingSegment();
            Assert.IsTrue(_lastSendingSegment3.Array != null);
            Assert.IsTrue(_lastSendingSegment3.Count == 1);
            Assert.AreEqual(_lastSendingSegment3.Array[_lastSendingSegment3.Offset], bytes4[3]);
        }

        /// <summary>
        /// channel测试代理
        /// </summary>
        class TempChannel : Channel {

            public TempChannel()
            {
                var _buffer = new BufferManager(4096);
                _buffer.Initialize();
                Buffer = _buffer;
                
            }

            public void appendToSendingQueue(ArraySegment<byte> data) {
                AppendToSendingQueue(data);
            }

            public ArraySegment<byte> GetLastSendingSegment() {
                //return _lastSendingSegment;
                return new ArraySegment<byte>();
            }

            internal bool GetSendingSegment(out ArraySegment<byte> _segment)
            {
                throw new NotImplementedException();
            }

            internal void AppendToSendingQueue(ArraySegment<byte> data) { }
        }

    }
}
