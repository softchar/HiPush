using Hi.NetWork.Eventloops;
using Hi.NetWork.Socketing.ChannelPipeline;
using Hi.NetWork.Socketing.Channels;
using Hi.NetWork.Socketing.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Bootstrapping
{
    /*
     1，引导IEventloopGroup
     2，引导IChannel
     3，引导IPipeline
    */

    /// <summary>
    /// 服务端引导
    /// </summary>
    public class ServerBootstrap : AbstructBootstrap<ServerBootstrap, IChannel>
    {

        public ServerBootstrap()
        {
            
        }

        public Task BindAsync(EndPoint remote = null)
        {
            var channel = InitServerChannel();

            channel.BindAsync(remote);

            return null;
  
        }

        private IChannel InitServerChannel()
        {
            IChannel channel = NewChannelFactory();

            channel.Pipeline.AddLast("ServerAcceptor", new ServerChannelAcceptor(WorkGroup, this.SetPipeline, this.ChannelConfig));

            ServerGroup.Next().Register(channel);

            return channel;

        }

        
    }
}
