using Hi.NetWork.Eventloops;
using Hi.NetWork.Socketing.ChannelPipeline;
using Hi.NetWork.Socketing.Channels;
using Hi.NetWork.Socketing.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Bootstrapping
{
    public class AbstructBootstrap<TBootstrap,TChannel>
        where TBootstrap : AbstructBootstrap<TBootstrap, TChannel>
        where TChannel : IChannel
    {
        /// <summary>
        /// 执行服务端管理连接请求的EventloopGroup（只有服务端才有请求过来）
        /// </summary>
        protected IEventloopGroup ServerGroup;

        /// <summary>
        /// 执行管理Channel中的IO操作的EventloopGroup
        /// </summary>
        protected IEventloopGroup WorkGroup;

        /// <summary>
        /// 新建Channel的工厂
        /// </summary>
        protected Func<TChannel> NewChannelFactory;

        /// <summary>
        /// ChannelConfig
        /// </summary>
        protected ChannelConfig ChannelConfig;

        /// <summary>
        /// 配置Pipeline
        /// </summary>
        protected Action<IChannelPipeline> SetPipeline;

        public AbstructBootstrap() { }

        /// <summary>
        /// 设置EventloopGroup
        /// </summary>
        /// <param name="serverGroup">执行管理连接请求的EventloopGroup</param>
        /// <param name="workGroup">执行管理Channel中的IO操作的EventloopGroup</param>
        /// <returns></returns>
        public TBootstrap Group(IEventloopGroup serverGroup, IEventloopGroup workGroup)
        {
            this.ServerGroup = serverGroup;
            this.WorkGroup = workGroup;

            return (TBootstrap)this;
        }

        /// <summary>
        /// 执行管理Channel中的IO操作的EventloopGroup
        /// </summary>
        /// <param name="workGroup"></param>
        /// <returns></returns>
        public TBootstrap Group(IEventloopGroup workGroup)
        {
            this.WorkGroup = workGroup;

            return (TBootstrap)this;
        }

        /// <summary>
        /// 配置服务端管理连接的Channel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public TBootstrap Channel<T>()
           where T : TChannel, new()
        {
            NewChannelFactory = () => new T();
            return (TBootstrap)this;
        }

        /// <summary>
        /// ChannelConfig
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public TBootstrap Config(ChannelConfig config)
        {
            this.ChannelConfig = config;
            return (TBootstrap)this;
        }

        /// <summary>
        /// 通信Channel的Pipeline配置
        /// </summary>
        /// <param name="setPipeline"></param>
        /// <returns></returns>
        public TBootstrap Pipeline(Action<IChannelPipeline> setPipeline)
        {
            this.SetPipeline = setPipeline;

            return (TBootstrap)this;
        }

    }
}
