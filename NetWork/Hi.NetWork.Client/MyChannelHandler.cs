using Hi.NetWork.Protocols;
using Hi.NetWork.Socketing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Hi.NetWork.Socketing.Channels;
using Hi.NetWork.Buffer;
using Hi.NetWork.Socketing.ChannelPipeline;

namespace Hi.NetWork.Client
{
    public class MyChannelHandler : ChannelHandler
    {

        static int index = 0;
        static long totalsize = 0;
        static int totalcount = 0;
        static int sendCounter = 0;
        static object obj = new object();

        public MyChannelHandler()
        {
            System.Timers.Timer time = new System.Timers.Timer();
            time.Elapsed += Time_Elapsed; ;
            time.Interval = 1000;
            time.Start();
        }

        public override void OnChannelRegister(IChannelHandlerContext context)
        {
            context.fireChannelRegister();
        } 

        public override void OnChannelWrite(IChannelHandlerContext context, object message)
        {
            lock (obj)
            {
                Interlocked.Increment(ref sendCounter);
                Interlocked.Increment(ref totalcount);
            }

        }

        public override void OnChannelRead(IChannelHandlerContext context, object message)
        {
            //base.OnChannelRead(context, message);
        }

        public override void OnChannelActive(IChannelHandlerContext context)
        {
            

            Task.Factory.StartNew(() =>
            {
                var channel = context.Channel;

                var bytes = CreateBytes1024();

                for (int i = 0; i < 1000000000; i++)
                {
                    //var bytes = System.Text.Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
                    //var buf = context.Alloc.GetSmallBuff();
                    //var buf = new FixedLengthByteBuf();
                    //var buf = new FixedLengthByteBuf();
                    var buf = new FixedLengthByteBuf(1024);
                    buf.Write(bytes);
                    context.WriteAsync(buf);

                    //context.Channel.Send(buf);
                    //channel.Send(bytes);
                }

            });

        }


        private byte[] CreateBytes1024()
        {
            var bytes = new byte[1024];

            int offset = 0;
            while (true)
            {
                var guidBytes = Encoding.UTF8.GetBytes(new Guid().ToString());
                if (offset + guidBytes.Length >= 1024) break;
                System.Buffer.BlockCopy(guidBytes, 0, bytes, offset, guidBytes.Length);
                offset += guidBytes.Length;
            }

            return bytes;
        }

        private void Time_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("发送数据：{0},次数：{1}", totalsize, sendCounter);
            Interlocked.Exchange(ref sendCounter, 0);
        }

    }
}
