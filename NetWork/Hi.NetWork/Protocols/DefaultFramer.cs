using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.NetWork.Socketing;
using Hi.NetWork.Buffer;
using System.Threading;
using System.Collections;

namespace Hi.NetWork.Protocols {

    /// <summary>
    /// 默认协议
    /// </summary>
    public class DefaultFramer : IFramer {

        public DefaultFramer() {
        }

        private int _headerLength = sizeof(Int32);

        private Action<ArraySegment<byte>> unPacketedCompleted;
        private Action unPacketedFinished;

        private int _packageLength = 0;//header的长度

        private int _headerbytes = 0;//已读的heaer字节数

        private int _bufferIndex = 0;

        private byte[] _messageBuffer;

        public byte[] MessageBuffer { 

            get { return _messageBuffer; } 

        } 

        public int PackageLength { get { return _packageLength; } } 

        public int Headerbytes { get { return _headerbytes; } } 

        /// <summary>
        /// 拆包完成事件，已将数据拆分出来，此时应该做数据处理操作
        /// </summary>
        public Action<ArraySegment<byte>> UnPacketedCompleted
        {
            get { return unPacketedCompleted; }
            set { unPacketedCompleted = value; }
        }

        /// <summary>
        /// 拆包已结束事件，数据处理已完成、临时缓冲区被释放，标志位已恢复为初始状态，此时可以对原始包的缓冲区进行释放
        /// </summary>
        public Action UnPacketedFinished
        {
            get { return unPacketedFinished; }
            set { unPacketedFinished = value; }
        }

        /// <summary>
        /// 封包
        /// </summary>
        public IEnumerable<ArraySegment<byte>> Packet(ArraySegment<byte> data)
        {
            var length = data.Count;
            yield return new ArraySegment<byte>(new[] { (byte)length, (byte)(length >> 8), (byte)(length >> 16), (byte)(length >> 24) });
            yield return data;
        }

        /// <summary>
        /// 解包
        /// </summary>
        public void Unpacking(IEnumerable<ArraySegment<byte>> sgmts)
        {
            if (sgmts == null)
                return;

            foreach (var seg in sgmts)
            {
                parse(seg);
            }

        }

        public void Unpacking(ArraySegment<byte> sgmt)
        {
            if (sgmt == null) return;

            parse(sgmt);
        }

        public void parse(ArraySegment<byte> sgmt)
        {

            var segment = sgmt;

            var data = segment.Array;

            for (int i = segment.Offset, k = i + segment.Count; i < k; i++)
            {
                if (_headerbytes < _headerLength)
                {
                    _packageLength |= data[i] << (_headerbytes * 8);
                    ++_headerbytes;

                    if (_headerbytes == _headerLength)
                    {
                        if (_packageLength <= 0) throw new Exception("");
                        try
                        {
                            _messageBuffer = new byte[_packageLength];
                        }
                        catch (Exception)
                        {
                            return;
                        }
                    }
                }
                else
                {                   
                    int copyCnt = Math.Min(segment.Count + segment.Offset - i, _packageLength - _bufferIndex);
                    System.Buffer.BlockCopy(segment.Array, i, _messageBuffer, _bufferIndex, copyCnt);

                    //block.Return();

                    _bufferIndex += copyCnt;
                    i += copyCnt - 1;

                    if (_bufferIndex == _packageLength)
                    {
                        unPacketedCompleted?.Invoke(new ArraySegment<byte>(_messageBuffer, 0, _bufferIndex));

                        _messageBuffer = null;
                        _headerbytes = 0;
                        _packageLength = 0;
                        _bufferIndex = 0;

                        unPacketedFinished?.Invoke();

                        
                    } 
                } 
            } 
        }

        
    }
}
