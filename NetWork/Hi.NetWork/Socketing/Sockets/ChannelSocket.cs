using Hi.Infrastructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing.Sockets {

    /// <summary>
    /// 相关Socket的操作
    /// </summary>
    public class ChannelSocket {

        Socket socket;

        private SocketAsyncEventArgs sendingSocketAsyncEventArgs;
        private SocketAsyncEventArgs receivingSocketAsyncEventAregs;

        public ChannelSocket() {
            sendingSocketAsyncEventArgs.Completed += IO_Completed;
            receivingSocketAsyncEventAregs.Completed += IO_Completed;
        }

        private void IO_Completed(object sender, SocketAsyncEventArgs e) {
            if (e.SocketError != SocketError.Success) {
            }

        }

        public void OnAccept(Socket socket) {

            Ensure.IsNotNull(socket);

        }

        private void active(Socket socket) {

            this.socket = socket;

            sendingSocketAsyncEventArgs = new SocketAsyncEventArgs();
            receivingSocketAsyncEventAregs = new SocketAsyncEventArgs();

            sendingSocketAsyncEventArgs.Completed += IO_Completed;
            receivingSocketAsyncEventAregs.Completed += IO_Completed;

            receive();
        }

        private void receive() {

            
            if (!socket.ReceiveAsync(receivingSocketAsyncEventAregs)) {
                processReceive(receivingSocketAsyncEventAregs);
            }

        }

        private void processReceive(SocketAsyncEventArgs e) {

        }
    }
}
