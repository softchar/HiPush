using Hi.NetWork.Socketing.ChannelPipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Hi.NetWork.Socketing.Channels;
using Hi.NetWork.Eventloops;
using Hi.NetWork.Socketing.Sockets;

namespace Hi.NetWork.Socketing.Handlers
{


    public class ServerChannelAcceptor : ChannelHandler
    {
        IEventloopGroup Group;
        Action<IChannelPipeline> PipelineAction;
        ChannelConfig ChannelConfig;

        public ServerChannelAcceptor(IEventloopGroup group, Action<IChannelPipeline> pipelineAction, ChannelConfig config)
        {
            this.Group = group;
            this.PipelineAction = pipelineAction;
            this.ChannelConfig = config;
        }

        public override Task BindAsync(IChannelHandlerContext context, EndPoint remote)
        {
            context.Channel.DoBind(remote);
            return context.BindAsync(remote);
        }

        public override void OnChannelRead(IChannelHandlerContext ctx, object message)
        {
            var channel = (IChannel)message;

            //初始化新的Channel
            InitNewChannel(channel);

            ctx.fireChannelRead(message);
        }

        public void InitNewChannel(IChannel channel)
        {
            var loop = Group.Next();

            channel.SetConfig(ChannelConfig).SetPipeline(PipelineAction);

            //将Channel注册到Eventloop中
            loop.Register(channel);

            //激活channel
            channel.ActiveAsync();

        }
    }
}
