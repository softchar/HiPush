using Hi.NetWork.Buffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing
{

    /// <summary>
    /// Channel消息
    /// </summary>
    public class ChannelMessage
    {

        /// <summary>
        /// 缓冲区的引用
        /// </summary>
        private IByteBuffer _buffer;

        private ArraySegment<byte> _data;

        private bool _isReturn = false;

        public ArraySegment<byte> Data { get { return _data; } }

        /// <summary>
        /// 消息所在的缓冲区地址是否已返回给缓冲区
        /// </summary>
        public bool IsReturn { get { return _isReturn; } }

        public ChannelMessage() { }

        public ChannelMessage(IByteBuffer buffer, ArraySegment<byte> segment)
        { 

            _buffer = buffer;

            _data = segment;

        }

        public ChannelMessage(ArraySegment<Byte> segment)
        {
            _data = segment;
        }

        /// <summary>
        /// 更新数据源
        /// </summary>
        public void UpdateData(ArraySegment<byte> segment) {

            //替换数据源之前先返回申请的缓冲区地址
            release();

            _data = segment;

            _isReturn = false;

        }

        /// <summary>
        /// 返回申请的缓冲区地址，如果没有返回数据的地址
        /// </summary>
        public void Return() {

            //如果没有释放数据的地址则释放
            if (!_isReturn) release();

        }

        /// <summary>
        /// 返回申请的缓冲区地址
        /// </summary>
        private void release()
        {
            _isReturn = true;
        }

    }
}
