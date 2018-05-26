using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Hi.NetWork.Buffer;
using Hi.NetWork.Protocols;
using Hi.Infrastructure.NetWork;
using Hi.Infrastructure.Base;
using System.Net;

namespace Hi.NetWork.Socketing {
    public class ClientChannel : Channel
    {

        private IChannelPipeline pipeline;

        public ClientChannel(IChannelPipeline pipeline,IByteBuffer buffer, IFramer framer) 
            : base(buffer, framer)
        {
            this.pipeline = pipeline;
            base.SetSocket(SocketUtils.CreateSocket());

        }

        public ClientChannel RegisterChannelHandler(Action<IChannelPipeline> registerHandlerAction)
        {
            registerHandlerAction?.Invoke(pipeline);
            return this;
        }

        public void Connection(string IP, int port)
        {
            var connectionEventArgs = new SocketAsyncEventArgs();
            connectionEventArgs.Completed += processConnectioned;
            connectionEventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);

            if (!this.Socket.ConnectAsync(connectionEventArgs)) {
                processConnectioned(null, connectionEventArgs);
            }

        }

        private void processConnectioned(object sender, SocketAsyncEventArgs e)
        {
            var ctx = new ChannelPipelineContext();
            ctx.SetChannel(this);

            pipeline.OnConnected(ctx);
            Receiving();
        }
    }
}
