using Hi.NetWork.Socketing.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing {

    /// <summary>
    /// 通道上下文,
    /// </summary>
    public class ChannelContext {


        private IChannel _channel { get; set; }

        public IChannel Channel()
        {
            return _channel;
        }

        private IChannelPipeline _pipeline { get; set; }

        public IChannelPipeline Pipeline()
        {
            return _pipeline;
        }

        /// <summary>
        /// 获得所有连接的
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IChannel> Channels(Expression<Func<int>> conditions)
        {
            return null;
        }
        
    }
}
