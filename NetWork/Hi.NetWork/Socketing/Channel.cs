using Hi.Infrastructure.Base;
using Hi.NetWork.Buffer;
using Hi.NetWork.Protocols;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SysBuffer = System.Buffer;
using System.Diagnostics;

namespace Hi.NetWork.Socketing
{

    /// <summary>
    /// Channel通信通道,负责读写操作
    /// </summary>
    public class Channel : IChannel
    {

        private Socket _socket;
        public Socket Socket
        {
            get { return _socket; }
        }

        /// <summary>
        /// 是否已关闭
        /// </summary>
        protected bool _isClosed = true;

        /// <summary>
        /// 发送缓冲区的大小
        /// </summary>
        private int _sendingBufferSize = 8192;

        /// <summary>
        /// 发送缓冲区的大小
        /// </summary>
        public int SendingBufferSize
        {
            get { return _sendingBufferSize; }
            set { _sendingBufferSize = value; }
        }

        /// <summary>
        /// 接收缓冲区的大小
        /// </summary>
        protected int _receivingBufferSize = 8192;

        /// <summary>
        /// 接收缓冲区的大小
        /// </summary>
        public int ReceivingBufferSize
        {
            get { return _receivingBufferSize; }
            set { _receivingBufferSize = value; }
        }

        /// <summary>
        /// 共享缓冲区
        /// </summary>
        private IByteBuffer _buffer;

        /// <summary>
        /// 共享缓冲区
        /// </summary>
        public IByteBuffer Buffer
        {
            get { return _buffer; }
            set { _buffer = value; }
        }

        /// <summary>
        /// 帧处理器
        /// </summary>
        private IFramer _framer;

        /// <summary>
        /// 帧处理器
        /// </summary>
        protected IFramer Framer
        {
            get { return _framer; }
            set { _framer = value; }
        }

        /// <summary>
        /// 管道
        /// </summary>
        //public IChannelPipeline _pipeline;
        //public IChannelPipeline Pipeline
        //{
        //    get { return _pipeline; }
        //}

        ChannelSendingStream sendingStream;
        ChannelReceivingStream receivingStream;

        public Channel()
        {

        }

        public Channel(IByteBuffer buffer, IFramer frame)
        {
            //Ensure.IsNotNull(pipeline);
            Ensure.IsNotNull(buffer);
            Ensure.IsNotNull(frame);

            _buffer = buffer;

            _framer = new DefaultFramer();
            _framer.UnPacketedCompleted = ResolveCompleted;

        }

        /// <summary>
        /// 开始发送数据
        /// </summary>
        public void Send(byte[] data)
        {
            if (data == null || data.Length == 0) return;

            var segments = _framer.Packet(new ArraySegment<byte>(data, 0, data.Length));
            sendingStream.Send(segments);
        }

        /// <summary>
        /// 开始接收数据
        /// </summary>
        protected void Receiving()
        {
            receivingStream.StartReceiving();
        }

        private ChannelPipelineContext channelPipelineContext;

        /// <summary>
        /// 设置Socket,并且开始接收数据
        /// </summary>
        /// <param name="socket"></param>
        public void OnAccept(Socket socket)
        {
            SetSocket(socket);

            OnConnected();

            Receiving();
        }

        /// <summary>
        /// 包解析完成
        /// </summary>
        /// <param name="segment"></param>
        protected virtual void ResolveCompleted(ArraySegment<byte> segment)
        {
            OnReceived(new ChannelMessage(_buffer, segment));
        }

        protected virtual void OnConnected() {

        }

        protected virtual void OnReceived(ChannelMessage message) {

        }

        /// <summary>
        /// 设置连接Socket
        /// </summary>
        /// <param name="socket"></param>
        public void SetSocket(Socket socket)
        {
            _socket = socket;
            _isClosed = false;

            sendingStream = new ChannelSendingStream(socket, this._buffer, this._sendingBufferSize);
            receivingStream = new ChannelReceivingStream(socket, this._buffer, this._framer, this._receivingBufferSize);
        }

        /// <summary>
        /// 获得远程节点的信息
        /// </summary>
        /// <returns></returns>
        public string GetRemoteNode()
        {
            return _socket.RemoteEndPoint.ToString();
        }
    }
}
