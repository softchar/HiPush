using Hi.NetWork.Buffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.NetWork.Socketing.ChannelPipeline;
using Hi.Infrastructure.Base;
using System.Diagnostics;

namespace Hi.NetWork.Code
{
    /// <summary>
    /// 定长编码器
    /// </summary>
    public class LengthMessageEncoder : MessageEncoder<IByteBuf>
    {
        public LengthMessageEncoder() { }

        public override void Encode(IChannelHandlerContext context, IByteBuf buf, List<object> output)
        {
            Ensure.IsNotNull(output);

            //获取buf内数据的长度
            var length = buf.Readables();

            try
            {
                var tiny = context.Alloc.Buffer(4);
                output.Add(tiny.Write(length));
                output.Add(buf);
            }
            catch (Exception e)
            {
                throw;
            }

        }
    }
}

