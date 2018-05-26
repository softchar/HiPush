using Hi.NetWork.Socketing.ChannelPipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.NetWork.Buffer;
using Hi.Infrastructure.Base;
using System.Diagnostics;

namespace Hi.NetWork.Code
{
    public abstract class MessageDecoder<T> : ChannelHandler
    {
        public override void OnChannelRead(IChannelHandlerContext context, object message)
        {
            Ensure.IsNotNull(message);

            List<object> output = new List<object>();

            if (message is T)
            {
                try
                {
                    Decode(context, (T)message, output);
                }
                catch (IndexOutOfRangeException e)
                {
                    Trace.WriteLine($"MessageDecoder.IndexOutOfRangeException");
                }
            } 

            if (output != null)
            {
                for (int i = 0; i < output.Count; i++)
                {
                    object msg = output[i];
                    context.fireChannelRead(msg);
                    Return((T)msg);
                }
            }
            else
            {
                context.fireChannelRead(message);
            }

        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="message"></param>
        /// <param name="output"></param>
        protected abstract void Decode(IChannelHandlerContext ctx, T message, List<object> output);

        /// <summary>
        /// 资源释放
        /// </summary>
        /// <param name="message"></param>
        protected abstract void Return(T message);
    }
}
