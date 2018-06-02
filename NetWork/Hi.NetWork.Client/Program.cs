using Hi.NetWork.Buffer;
using Hi.NetWork.Protocols;
using Hi.NetWork.Socketing;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hi.NetWork.Channels;
using Hi.Infrastructure.Ioc;
using Hi.NetWork.Socketing.Channels;
using Hi.Infrastructure.Configuration;
using Hi.NetWork.Configuration;
using Hi.NetWork.Eventloops;
using Hi.NetWork.Socketing.ChannelPipeline;
using Hi.NetWork.Code;
using Hi.NetWork.Bootstrapping;
using Hi.NetWork.Socketing.Sockets;

namespace Hi.NetWork.Client {
    class Program {

        static void Main(string[] args) {

            CreateClient();

            Console.ReadKey();

        }

        private static void CreateClient()
        {

            var channelConfig = new ChannelConfig()
            {
                AutoReceiving = true,
                PenddingMessageCounter = 102400,
                ReceivingBufferSize = 1024 * 64,
                SendingBufferSize = 1024 * 64
            };


            var workGroup = new MutlEventloopGroup(1);

            var bootstrap = new ClientBootstrap();
            bootstrap
                .Group(workGroup)
                .Channel<TcpClientChannel>()
                .Config(channelConfig)
                .Pipeline(pipeline => 
                {
                    pipeline.AddLast("Tls", new TlsHandler());
                    pipeline.AddLast("Enc", new LengthMessageEncoder());
                    pipeline.AddLast("Dec", new LengthMessageDecoder());
                    pipeline.AddLast("MyChannelHandler", new MyChannelHandler());
                });
            bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("192.168.1.103"), 46456));

        }

    }
}
