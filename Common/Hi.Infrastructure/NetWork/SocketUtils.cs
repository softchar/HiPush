using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.NetWork {
    public class SocketUtils {
        public static Socket CreateSocket() {

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Blocking = false;
            socket.NoDelay = true;

            //socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Linger, true);

            return socket;
        }
    }
}
