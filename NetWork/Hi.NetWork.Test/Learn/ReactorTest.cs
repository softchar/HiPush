using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Net.WebSockets;

namespace Hi.NetWork.Test.Learn {
    [TestClass]
    public class ReactorTest {

        [TestMethod]
        public void start() {

            IEventHandler client1 = new MessageEventHandler(IPAddress.Parse("123.123.123.123"), 123);
            IEventHandler client2 = new MessageEventHandler(IPAddress.Parse("234.234.234.234"), 123);
            IEventHandler client3 = new MessageEventHandler(IPAddress.Parse("235.235.235.235"), 123);

            ISyncEventDemultiplexer synchronousEventDemultiplexer = new SyncEventDemultiplexer();

            Reactor dispatcher = new Reactor(synchronousEventDemultiplexer);

            dispatcher.RegisterHandle(client1);
            dispatcher.RegisterHandle(client2);
            dispatcher.RegisterHandle(client3);

            dispatcher.HandleEvents();

        }


    }

    interface IEventHandler {
        void HandleEvent(byte[] data);
        TcpListener GetHandler();


    }
    interface ISyncEventDemultiplexer {
        IList<TcpListener> Select(ICollection<TcpListener> listeners);
    }

    interface IReactor {
        void RegisterHandle(IEventHandler eventHandler);
        void RemoveHandle(IEventHandler eventHandler);
        void HandleEvents();
    }

    class MessageEventHandler : IEventHandler {

        private readonly TcpListener _listener;

        public MessageEventHandler(IPAddress ipAddress, int port) {
            _listener = new TcpListener(ipAddress, port);
        }

        public TcpListener GetHandler() {
            return _listener;
        }

        public void HandleEvent(byte[] data) {
            string message = Encoding.UTF8.GetString(data);
        }
    }

    class SyncEventDemultiplexer : ISyncEventDemultiplexer {
        public IList<TcpListener> Select(ICollection<TcpListener> listeners) {
            var tcpListeners = new List<TcpListener>(from listener in listeners where listener.Pending() select listener);
            return tcpListeners;
        }
    }

    class Reactor : IReactor {
        private readonly ISyncEventDemultiplexer _synchronousEventDemultiplexer;
        private readonly IDictionary<TcpListener, IEventHandler> _handlers;

        public Reactor(ISyncEventDemultiplexer synchronousEventDemultiplexer) {
            _synchronousEventDemultiplexer = synchronousEventDemultiplexer;
            _handlers = new Dictionary<TcpListener, IEventHandler>();
        }

        public void RegisterHandle(IEventHandler eventHandler) {
            _handlers.Add(eventHandler.GetHandler(), eventHandler);
        }

        public void RemoveHandle(IEventHandler eventHandler) {
            _handlers.Remove(eventHandler.GetHandler());
        }

        public void HandleEvents() {
            while (true) {
                IList<TcpListener> listeners = _synchronousEventDemultiplexer.Select(_handlers.Keys);

                foreach (TcpListener listener in listeners) {
                    int dataReceived = 0;
                    byte[] buffer = new byte[1];
                    IList<byte> data = new List<byte>();

                    Socket socket = listener.AcceptSocket();

                    do {
                        dataReceived = socket.Receive(buffer);

                        if (dataReceived > 0) {
                            data.Add(buffer[0]);
                        }

                    } while (dataReceived > 0);

                    socket.Close();

                    _handlers[listener].HandleEvent(data.ToArray());
                }
            }
        }
    }

}
