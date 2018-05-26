using Hi.NetWork.Socketing.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.NetWork.Buffer;
using System.Net;
using System.Net.Sockets;
using Hi.NetWork.Eventloops;

namespace Hi.NetWork.Socketing.Sockets
{
    public class TcpClientChannel : AbstractChannel
    {
        ChannelSocketAsyncEventArgs connectEventArgs;

        private Socket socket;
        public Socket Socket
        {
            get { return socket; }
        }

        public TcpClientChannel()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                Blocking = false,
                NoDelay = true
            };
            connectEventArgs = new ChannelSocketAsyncEventArgs();
            connectEventArgs.Completed += IO_Completed;
        }

        public IChannel NewChannelFactory(Socket socket) => new TcpSocketChannel(socket);

        private void IO_Completed(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.LastOperation == SocketAsyncOperation.Connect)
            {
                ProcessConnect((ChannelSocketAsyncEventArgs)args);
            }
            else
            {
                //连接失败
            }
        }

        private void ProcessConnect(ChannelSocketAsyncEventArgs args)
        {
            var channel = NewChannelFactory(args.ConnectSocket);
            invoker.fireOnChannelRead(channel);
        }
        public override Task BindAsync(EndPoint address = null)
        {
            throw new NotImplementedException();
        }

        public override Task ConnectAsync(EndPoint remote = null)
        {
            return invoker.fireConnectAsync(remote);
        }

        public override Task DoBind(EndPoint address = null)
        {
            throw new NotImplementedException();
        }

        public override Task DoConnect(EndPoint remote = null)
        {
            var promise = new TaskCompletionSource();

            Execute(() =>
            {
                connectEventArgs.RemoteEndPoint = remote;

                if (!Socket.ConnectAsync(connectEventArgs))
                {
                    ProcessConnect(connectEventArgs);
                }

                promise.Success();

            });

            return promise.Task;
        }

        public override EndPoint GetRemoteNode()
        {
            throw new NotImplementedException();
        }

        public override IChannel SetConfig(ChannelConfig config)
        {
            throw new NotImplementedException();
        }

        public override Task WriteAsync(IByteBuf buf)
        {
            throw new NotImplementedException();
        }
    }
}
