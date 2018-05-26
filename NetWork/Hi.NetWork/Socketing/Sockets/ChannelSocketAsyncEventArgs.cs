using Hi.Infrastructure.Base;
using Hi.NetWork.Buffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing.Sockets
{
    public class ChannelSocketAsyncEventArgs : SocketAsyncEventArgs
    {
        public IByteBuf ByteBuf { get; set; }

    }
}
