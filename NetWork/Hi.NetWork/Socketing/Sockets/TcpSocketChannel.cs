using Hi.NetWork.Socketing.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Hi.NetWork.Buffer;
using System.Net.Sockets;
using Hi.NetWork.Eventloops;

namespace Hi.NetWork.Socketing.Sockets
{
    public class TcpSocketChannel : AbstractSocketChannel
    {
        public TcpSocketChannel(Socket socket)
            : base(socket)
        {
            
        }

        protected override IChannelInvoker NewChannelInvoker()
        {
            return new TcpChannelInvoker(this);
        } 

        public override Task BindAsync(EndPoint address = null)
        {
            throw new NotImplementedException();
        }

        public override Task ConnectAsync(EndPoint remote = null)
        {
            throw new NotImplementedException();
        }

        public override Task DoBind(EndPoint address = null)
        {
            throw new NotImplementedException();
        }

        public override Task DoConnect(EndPoint remote = null)
        {
            throw new NotImplementedException();
        }

        class TcpChannelInvoker : ChannelInvoker
        {
            public TcpChannelInvoker(AbstractChannel parent) : base(parent)
            {
                
            }

            protected override IByteBufAllocator NewByteBufAlloc()
            {
                return /*PooledThreadLocalBytebufAlloctor.Value*/ DefaultByteBufAllocator.Default;
            }
        }
    }
}
