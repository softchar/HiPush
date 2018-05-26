using Hi.NetWork.Socketing.ChannelPipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Hi.NetWork.Eventloops;
using Hi.NetWork.Socketing.Channels;
using Hi.NetWork.Socketing.Sockets;

namespace Hi.NetWork.Socketing.Handlers
{
    public class ClientChannelConnector : ChannelHandler
    {
        IEventloopGroup Group;
        Action<IChannelPipeline> PipelineAction;
        ChannelConfig ChannelConfig;

        public ClientChannelConnector(IEventloopGroup group, Action<IChannelPipeline> pipelineAction, ChannelConfig channelConfig)
        {
            this.Group = group;
            this.PipelineAction = pipelineAction;
            this.ChannelConfig = channelConfig;
        }

        public override Task ConnectAsync(IChannelHandlerContext context, EndPoint remote)
        {
            context.Channel.DoConnect(remote);
            return context.ConnectAsync(remote);
        }

        public override void OnChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is IChannel)
            {
                IChannel channel = message as IChannel;

                var loop = Group.Next();

                channel.SetConfig(ChannelConfig).Pipeline.SetAlloc(loop.Alloc);

                //配置Pipeline
                channel.SetPipeline(PipelineAction);

                //将channel注册到Eventloop
                loop.Register(channel);

                //异步执行激活
                channel.ActiveAsync();
            }

        }
    }
}
