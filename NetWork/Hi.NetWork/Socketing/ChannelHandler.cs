using Hi.NetWork.Socketing.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing {
    public class ChannelHandler : IChannelHandler {

        private IChannelHandler _next;

        public ChannelHandler() { }

        public virtual void SetNext(IChannelHandler next) {
            _next = next;
        }

        public virtual bool OnConnected(IChannel channel) {
            return true;
        }

        public virtual bool OnReceived(IChannel channel, ChannelMessage message) {
            return true;
        }

        public virtual bool OnSend(IChannel channel) { return true; }

        public virtual bool OnBreaked() { return true; }

        public virtual bool OnReconnected() { return true; }

        public IChannelHandler Next() {
            return _next;
        }

        
    }
   
}
