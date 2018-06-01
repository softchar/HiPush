using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Hi.NetWork.Socketing.Sockets
{
    using Channels;
    using Infrastructure.Base;
    using System.Net;
    using System.Net.Sockets;
    using Buffer;

    /// <summary>
    /// 接收器
    /// </summary>
    public class TcpServerChannel : AbstractChannel
    {
        private Semaphore semaphore;
        private Socket socket;
        private AcceptConfig config;
        private ChannelConfig channelConfig;

        public Socket Socket
        {
            get { return socket; }
        }

        public AcceptConfig Config
        {
            get { return config; }
        }

        public TcpServerChannel()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                Blocking = false,
                NoDelay = true
            };

            this.config = new AcceptConfig();

            this.semaphore = new Semaphore(config.MaxConcurrentNumber, config.MaxConcurrentNumber);

        }

        public IChannel NewChannelFactory(Socket socket) => new TcpSocketChannel(socket);

        /// <summary>
        /// 设置配置信息
        /// </summary>
        /// <param name="config"></param>
        public void SetConfig(AcceptConfig config)
        {
            this.config = config;
        }

        public override IChannel SetConfig(ChannelConfig config)
        {
            channelConfig = config;
            return this;
        }

        public void DoBind(string IP = null, int port = 0)
        {
            Ensure.CompareExchange(ref IP, config.Host);
            Ensure.CompareExchange(ref port, config.Port, 0);

            socket.Bind(new IPEndPoint(IPAddress.Parse(IP), port));
            socket.Listen(config.SocketLinsenQueueLength);

            StartAccept(null);

        }

        /// <summary>
        /// 开始异步等待请求
        /// </summary>
        /// <param name="socketAsyncEventArgs"></param>
        private void StartAccept(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            Execute(() => 
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
                semaphore.WaitOne();

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
                    Shutdown();
                }
            });

        }

        private void ProcessAccept(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    var channel = NewChannelFactory(e.AcceptSocket);
                    invoker.fireOnChannelRead(channel);
                }

            }
            catch (Exception ex)
            {
                Shutdown();
            }
            finally
            {
                e.AcceptSocket = null;
                StartAccept(e);
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        private void Shutdown()
        {
            socket.Close();
        }

        public override EndPoint GetRemoteNode()
        {
            throw new NotImplementedException();
        }

        public override Task BindAsync(EndPoint address = null)
        {
            if (address == null)
            {
                address = new IPEndPoint(IPAddress.Parse(config.Host), config.Port);
            }
            return invoker.fireBindAsync(address);
        }

        public override Task ConnectAsync(EndPoint remote = null)
        {
            throw new NotImplementedException();
        }

        public override Task DoBind(EndPoint address = null)
        {
            if (address == null)
            {
                address = new IPEndPoint(IPAddress.Parse(config.Host), config.Port);
            }

            try
            {
                socket.Bind(address);
                socket.Listen(config.SocketLinsenQueueLength);

                Execute(() => { StartAccept(null); });
            }
            catch (Exception e)
            {
                return null;
            }

            return null;
        }

        public override Task DoConnect(EndPoint remote = null)
        {
            throw new NotImplementedException();
        }

        public override Task WriteAsync(IByteBuf buf)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 接收器配置
    /// </summary>
    public class AcceptConfig
    {
        /// <summary>
        /// 最大并发数
        /// </summary>
        public int MaxConcurrentNumber = 65535;

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int MaxConnectionNumber = 300000;

        /// <summary>
        /// 监听Socket的处理队列长度
        /// </summary>
        public int SocketLinsenQueueLength = 5000;

        /// <summary>
        /// 监听的地址
        /// </summary>
        public string Host = "127.0.0.1";

        /// <summary>
        /// 监听的端口
        /// </summary>
        public int Port = 51410;

    }
}
