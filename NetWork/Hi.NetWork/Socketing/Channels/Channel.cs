
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing.Channels {

    using Hi.Infrastructure.Base;
    using Hi.NetWork.Eventloops;
    using Hi.NetWork.Socketing.Sockets;
    using Hi.NetWork.Buffer;
    using Hi.NetWork.Protocols;

    /// <summary>
    /// Channel通信通道,负责读写操作
    /// </summary>
    public class Channel : ChannelSocket, IChannel {

        private Socket _socket;                     //Socket 
        protected bool _isClosed = true;            //是否已关闭 
        private ChannelConfig config;
        private IFramer _framer;                    //帧处理器 
        private IByteBuffer _buffer;                //字节缓冲区 

        private int sendPedding = 0;
        private int receivePedding = 0;

        private ChannelOutput sendingStream;
        private ChannelInput receivingStream;

        public Socket Socket {
            get { return _socket; }
        }

        /// <summary>
        /// 发送缓冲区的大小
        /// </summary>
        public int SendingBufferSize {
            get { return config.SendingBufferSize; }
        }

        /// <summary>
        /// 接收缓冲区的大小
        /// </summary>
        public int ReceivingBufferSize {
            get { return config.ReceivingBufferSize; }
        }

        /// <summary>
        /// 帧处理器
        /// </summary>
        protected IFramer Framer {
            get { return _framer; }
            set { _framer = value; }
        }

        private Eventloop eventloop;

        private IChannelPipeline pipeline;

        public Eventloop Eventloop {
            get {
                return eventloop;
            }
        }

        public Channel() {
            config = new ChannelConfig();
        }

        public Channel(IByteBuffer buffer, IFramer framer, IChannelPipeline pipeline) {
            Ensure.IsNotNull(buffer);
            Ensure.IsNotNull(framer);
            Ensure.IsNotNull(pipeline);

            _buffer = buffer;
            _framer = framer;
            _framer.UnPacketedCompleted = ResolveCompleted;

            this.pipeline = pipeline;
            this.pipeline.SetChannel(this);

            config = new ChannelConfig();

        }

        /// <summary>
        /// 讲Channel关联到Eventloop
        /// </summary>
        /// <param name="loop"></param>
        public void AssociateToEventloop(Eventloop loop) {
            this.eventloop = loop;

            execute(() => {
                this.pipeline.OnRegisteredEventloop();
                onAccept();
            });
        }

        /// <summary>
        /// 开始发送数据
        /// </summary>
        public void Send(byte[] data) {
            if (data == null || data.Length == 0) return;


            //execute(() => {
                var segments = _framer.Packet(new ArraySegment<byte>(data, 0, data.Length));
                sendingStream.Send(segments);
            //});
        }

        /// <summary>
        /// 开始接收数据
        /// </summary>
        protected void Receiving() {
            receivingStream.StartReceiving();
        }

        /// <summary>
        /// 设置Socket,并且开始接收数据
        /// </summary>
        /// <param name="socket"></param>
        public void OnAccept(Socket socket) {
            SetSocket(socket);
            onAccept();
        }

        public void onAccept() =>
            execute(() => {
                this.pipeline.OnConnected();
                Receiving();
            });


        /// <summary>
        /// 包解析完成
        /// </summary>
        /// <param name="segment"></param>
        protected virtual void ResolveCompleted(ArraySegment<byte> segment) =>
            execute(() => {
                var message = new ChannelMessage(segment);
                this.pipeline.OnReceived(message);
            });

        

        /// <summary>
        /// 设置连接Socket
        /// </summary>
        /// <param name="socket"></param>
        public void SetSocket(Socket socket) {
            _socket = socket;
            _isClosed = false;

            sendingStream = new ChannelOutput(socket, this._buffer, config.SendingBufferSize);
            sendingStream.SendFinished = () => { onSendFinished(); };
            receivingStream = new ChannelInput(socket, this._buffer, this._framer, config.ReceivingBufferSize);
        }

        /// <summary>
        /// 设置配置信息
        /// </summary>
        /// <param name="config"></param>
        public void SetConfig(ChannelConfig config) {
            this.config = config;
        }

        /// <summary>
        /// 获得远程节点的信息
        /// </summary>
        /// <returns></returns>
        public string GetRemoteNode() {
            return _socket.RemoteEndPoint.ToString();
        }

        protected void execute(Action action) {
            Ensure.IsNotNull(action);

            if (eventloop.InEventloop) {
                action();
            } else {
                eventloop.Execute(new ActionTask(action));
            }

        }

        protected  void onSendFinished() => execute(() => {
                this.pipeline.OnSend();
            });
        }
        
    }

    public class ChannelConfig {
        public int SendingBufferSize = 8192;        //发送缓冲区的大小
        public int ReceivingBufferSize = 8192;      //接收缓冲区的大小
    }
}
