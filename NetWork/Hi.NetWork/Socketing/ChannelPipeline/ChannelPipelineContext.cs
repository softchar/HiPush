using Hi.NetWork.Pipeline;
using Hi.NetWork.Socketing.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing {
    public class ChannelPipelineContext : PipelineContext {

        private IChannel channel = null;

        public void SetChannel(IChannel channel) {
            this.channel = channel;
        }

        public IChannel Channel {
            get { return channel; }
        }

    }
}
