using Hi.Infrastructure.Base;
using Hi.NetWork.Buffer;
using Hi.NetWork.Socketing.ChannelPipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Code
{
    public abstract class MessageEncoder<T> : ChannelHandler
    {
        public override Task WriteAsync(IChannelHandlerContext context, object data)
        {
            Ensure.IsNotNull(data);

            Task result = null;

            List<object> output = new List<object>();

            var msg = (T)data;

            Encode(context, msg, output);

            if (output.Count > 0)
            {
                int lastIndex = output.Count - 1;
                for (int i = 0; i < lastIndex; i++)
                {
                    result = context.WriteAsync(output[i]);
                }
                result = context.WriteAsync(output[lastIndex]);
            }
            //else
            //{ 
            //    result = context.WriteAsync(DefaultByteBufAllocator.Empty);
            //} 

            return result;

        }

        public abstract void Encode(IChannelHandlerContext context, T data, List<object> output);
    }
}