using Hi.Infrastructure.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hi.NetWork.Protocols;
using Hi.Infrastructure.Configuration;
using Hi.NetWork.Server.Server;
using Hi.NetWork.Server.Model;
using Hi.Infrastructure.Base;
using System.Diagnostics;
using System.Threading;
using Hi.NetWork.Eventloops;
using Newtonsoft.Json;
using Hi.NetWork.Buffer;
using Hi.NetWork.Socketing.ChannelPipeline;
using Hi.NetWork.Code;
using Hi.NetWork.Bootstrapping;
using Hi.NetWork.Socketing.Sockets;
using System.Net;

namespace Hi.NetWork.Server
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

       

        int readTimes;                  //接收数据的总次数

        int tps;                        //每秒处理的次数
        int avgtps;                     //平均每秒处理的次数

        double totalTransDataSize;      //接收总字节数
        double secondTransDataSize;     //每秒接收的数据

        int bps;                        //每秒处理的字节数
        int avgbps;                     //平均每秒处理的字节数
        
        int timers; //秒数

        ServerBootstrap bootstrap;

        public MainWindow()
        {

            //abc
            InitializeComponent();

            ChannelConfig channelConfig = new ChannelConfig()
            {
                ReceivingBufferSize = 1024 * 4,
                SendingBufferSize = 1024 * 4,
                PenddingMessageCounter = 1024,
                AutoReceiving = true,
            };

            var serverGroup = new MutlEventloopGroup(1);
            var workGroup = new MutlEventloopGroup();

            bootstrap = new ServerBootstrap();
            bootstrap.Group(serverGroup, workGroup)
                .Channel<TcpServerChannel>()
                .Config(channelConfig)
                .Pipeline(pipeline =>
                {
                    pipeline.AddLast("Tls", new TlsHandler());
                    pipeline.AddLast("Enc", new LengthMessageEncoder());
                    pipeline.AddLast("Dec", new LengthMessageDecoder());
                    pipeline.AddLast("MyChannelHandler", new MyChannelHandler(this));
                    pipeline.AddLast("PerformanceHandler", new PerformanceHandler());
                });
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            bootstrap.BindAsync(new IPEndPoint(IPAddress.Parse("192.168.1.101"), 51410));

            System.Timers.Timer time = new System.Timers.Timer();
            time.Elapsed += Time_Elapsed;
            time.Interval = 1000;
            time.Start();

        }

        private void Time_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            
            this.Dispatcher.Invoke(() => 
            {
                if (timers > 0)
                {
                    lblTPSValue.Content = tps;
                    lblAverageTPSValue.Content = readTimes / timers;
                    lblTotalReceivedCountValue.Content = Math.Round(totalTransDataSize / 1024 / 1024, 3) + "m";
                    lblBPSValue.Content = Math.Round(secondTransDataSize / 1024 / 1024, 3) + "m/s";
                    lblAverageBPSValue.Content = Math.Round(totalTransDataSize / timers / 1024 / 1024, 3) + "m/s";
                }
                
            });
            
            Interlocked.Increment(ref timers);
            Interlocked.Exchange(ref tps, 0);
            Interlocked.Exchange(ref secondTransDataSize, 0);
        }

        class MyChannelHandler : ChannelHandler
        {

            MainWindow _parent;
            ReceiveLogServer _server;
            //Dispatcher _dispatcher ;
            DefaultProtocol _protocol = new DefaultProtocol();

            object _sync = new object();

            public MyChannelHandler(MainWindow wind)
            {

                _parent = wind;
                _server = new ReceiveLogServer();
                //_dispatcher = new Dispatcher();
                _protocol = new DefaultProtocol();

                //_dispatcher.Subscrible(EventType.Login, Login);

            }

            public override void OnChannelActive(IChannelHandlerContext context)
            {
                Ensure.IsNotNull(context);

                _parent.Dispatcher.Invoke(() =>
                {

                    //_parent.lstOnline.Items.Add(channel.GetRemoteNode());
                    // _parent.lblStatus.Content = "在线";

                    //_parent.lstOnline.Items.Insert(0, channel.GetRemoteNode());

                });
            }

            public override void OnChannelRead(IChannelHandlerContext context, object message)
            {
                Ensure.IsNotNull(context);
                Ensure.IsNotNull(message);

                var buf = (IByteBuf)message;

                lock (_sync)
                {
                    
                    _parent.readTimes++;
                    _parent.tps++;
                    _parent.totalTransDataSize += buf.Readables();
                    _parent.secondTransDataSize += buf.Readables();
                }

                //int size = buf.Readables();
                //if (size == 0)
                //{
                //    Trace.WriteLine($"MyChannelHandler.OnChannelRead");
                //}
                //else
                //{
                //    var buf2 = context.Alloc.Buffer(buf.Readables());
                //    buf2.Write(buf);
                //    context.WriteAsync(buf2);
                //}
            }

        }

        class PerformanceHandler : ChannelHandler
        {
            int number = 0;
            //long totalSize = 0;

            System.Timers.Timer time = null;

            public PerformanceHandler()
            {
                
            }

            ~PerformanceHandler()
            {
                
            }

            private void Time_Elapsed1(object sender, System.Timers.ElapsedEventArgs e)
            {
                //Trace.WriteLine("每秒处理数据:" + number);

                Interlocked.Exchange(ref number, 0);

            }

            public override void OnChannelRead(IChannelHandlerContext context, object message)
            {

                Ensure.IsNotNull(context);
                Ensure.IsNotNull(message);

                Interlocked.Increment(ref number);
            }
        }
    }
}
