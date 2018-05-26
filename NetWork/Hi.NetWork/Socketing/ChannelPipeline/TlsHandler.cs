using Hi.NetWork.Buffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing.ChannelPipeline
{
    public class TlsHandler : ChannelHandler
    {
        public override Task WriteAsync(IChannelHandlerContext ctx, object message)
        {
            IByteBuf buf = null;

            if (message is IByteBuf)
            {
                buf = (IByteBuf)message;
            }
            else if (message is byte[])
            {
                var bytes = (byte[])message;
                buf = ctx.Alloc.Buffer(bytes.Length);
                buf.Write(bytes);
            }
            else if (message is ArraySegment<Byte>)
            {
                var segment = (ArraySegment<Byte>)message;
                buf = ctx.Alloc.Buffer(segment.Count);
                buf.Write(segment.Array, segment.Offset, segment.Count);
            }

            return ctx.Channel.WriteAsync(buf);
        }
    }
}
