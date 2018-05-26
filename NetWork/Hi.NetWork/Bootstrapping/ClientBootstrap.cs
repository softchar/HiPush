using Hi.NetWork.Socketing.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Hi.NetWork.Socketing.Channels;
using Hi.NetWork.Socketing.Handlers;

namespace Hi.NetWork.Bootstrapping
{
    public class ClientBootstrap : AbstructBootstrap<ClientBootstrap, TcpClientChannel>
    {
        public void ConnectAsync(IPEndPoint iPEndPoint)
        {
            var channel = InitClientChannel();

            channel.ConnectAsync(iPEndPoint);

        }

        private IChannel InitClientChannel()
        {
            IChannel channel = NewChannelFactory();

            channel.Pipeline.AddLast("ClientConnector", new ClientChannelConnector(WorkGroup, SetPipeline, this.ChannelConfig));

            WorkGroup.Next().Register(channel);

            return channel;

        }
    }
}
