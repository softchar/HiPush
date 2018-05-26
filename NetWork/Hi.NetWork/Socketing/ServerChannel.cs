using Hi.Infrastructure.EventHandle;
using Hi.Infrastructure.NetWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hi.NetWork.Buffer;
using Hi.NetWork.Sessions;
using Hi.NetWork.Configuration;
using Hi.NetWork.Protocols;
using Hi.Infrastructure.Base;
using Hi.Infrastructure.Ioc;

namespace Hi.NetWork.Socketing {

    /// <summary>
    /// 服务端Socket
    /// </summary>
    public class ServerChannel : Channel {

        /// <summary>
        /// 信号量，用于控制最大并发数
        /// </summary>
        private Semaphore _semaphore;

        /// <summary>
        /// 连接池
        /// </summary>
        private SessionPool _sessionPool;

        /// <summary>
        /// 配置
        /// </summary>
        private ConfigurationSetting _setting;

        private IContainer _container;

        private IChannelPipeline pipeline;

        /// <summary>
        /// 在线会话
        /// </summary>
        private Session _session;

        private Action<IChannelPipeline> _channelPipelineFactory;
        private IChannelPipeline createPipeline() {

            var _channelPipeline = _container.Get<IChannelPipeline>();

            _channelPipelineFactory?.Invoke(_channelPipeline);

            return _channelPipeline;

        }

        public ServerChannel(IContainer container, IChannelPipeline pipeline, IByteBuffer buffer, IFramer framer, ConfigurationSetting setting) 
            : base(buffer, framer) {

            Ensure.IsNotNull(container);
            Ensure.IsNotNull(pipeline);
            Ensure.IsNotNull(buffer);
            Ensure.IsNotNull(framer);

            SendingBufferSize = setting.SocketSendBufferSize;
            ReceivingBufferSize = setting.SocketReceiveBufferSize;

            _container = container;
            _setting = setting;

            this.pipeline = pipeline;

            _channelPipelineFactory?.Invoke(pipeline);

            SetSocket(SocketUtils.CreateSocket());



        }

        /// <summary>
        /// 初始化（会话池，最大连接数）
        /// </summary>
        public void initialize()
        {

            _semaphore = new Semaphore(_setting.MaxConcurrentNumber, _setting.MaxConcurrentNumber);

            _sessionPool = new SessionPool(_setting.MaxConcurrentNumber);

            for (int i = 0; i < _setting.MaxConcurrentNumber; i++)
            {
                //var _channel = new Channel(createPipeline(), this.Buffer, _container.Get<IFramer>());
                var framer = _container.Get<IFramer>();
                var channel = new Channel(this.Buffer, framer);

                _sessionPool.Push(channel);
            }

            _session = new Session();

            Console.WriteLine("初始化完毕...");

        }

        /// <summary>
        /// 向Pipeline管道中注册ChannelHandler
        /// </summary>
        /// <param name="registerHandlerAction"></param>
        /// <returns></returns>
        public ServerChannel RegisterChannelHandler(Action<IChannelPipeline> registerHandlerAction)
        {

            _channelPipelineFactory = registerHandlerAction;

            return this;

        }

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="port"></param>
        /// <param name="backlog"></param>
        public void DoBind(string IP = null, int port =0 , int backlog = 0)
        {
            
            Ensure.CompareExchange(ref IP, _setting.Host);
            Ensure.CompareExchange(ref port, _setting.Port, 0);
            Ensure.CompareExchange(ref backlog, _setting.SocketLinsenQueueLength, 0);

            initialize();

            base.Socket.Bind(new IPEndPoint(IPAddress.Parse(IP), port));
            base.Socket.Listen(backlog);

            StartAccept(null);
        }

        /// <summary>
        /// 开始异步等待请求
        /// </summary>
        /// <param name="socketAsyncEventArgs"></param>
        private void StartAccept(SocketAsyncEventArgs socketAsyncEventArgs)
        {

            if (socketAsyncEventArgs == null)
            {
                socketAsyncEventArgs = new SocketAsyncEventArgs();
                socketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ProcessAccept);
            }
            else
            {
                socketAsyncEventArgs.AcceptSocket = null;
            }

            //等待信号
            _semaphore.WaitOne();

            try
            {
                //开始异步等待连接
                if (!Socket.AcceptAsync(socketAsyncEventArgs))
                {
                    ProcessAccept(null, socketAsyncEventArgs);
                }
            }
            catch (Exception)
            {
                //ShutDownSocket();
            }
        }

        private void ProcessAccept(object sender, SocketAsyncEventArgs e)
        {

            try
            {
                if (e.SocketError == SocketError.Success)
                {

                    //从会话连接池获取一个会话对象，避免创建对象和垃圾回收造成的开销
                    var _channel = _sessionPool.Pop();
                    
                    //通信开始
                    _channel.OnAccept(e.AcceptSocket);

                    //将会话保存到在线列表中
                    _session.Add(_channel);

                }

            } catch (Exception excep)
            {
            } finally
            {

                e.AcceptSocket = null;
                StartAccept(e);

            }
        }

        protected override void OnConnected() {

            var ctx = new ChannelPipelineContext();
            ctx.SetChannel(this);

            pipeline.OnConnected(ctx);
        }

        protected override void OnReceived(ChannelMessage message) {

            var ctx = new ChannelPipelineContext();
            ctx.SetChannel(this);

            pipeline.OnReceived(ctx, message);
        }

    }
}
