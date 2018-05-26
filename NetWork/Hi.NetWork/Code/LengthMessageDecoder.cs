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
    public class LengthMessageDecoder : MessageDecoder<IByteBuf>
    {
        static int LengthFrame = sizeof(Int32);

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="buf"></param>
        /// <param name="output"></param>
        protected override void Decode(IChannelHandlerContext ctx, IByteBuf buf, List<object> output)
        {
            Ensure.IsNotNull(buf);

            //buf的可读字节数
            int read = buf.Readables();

            for (int i = 0; i < read; i++)
            {

                if (ctx.IncompleteLength < LengthFrame)
                {
                    ctx.IncompleteHeader |= buf.ReadByte() << ctx.IncompleteLength * 8;
                    ctx.IncompleteLength++;

                    if(ctx.IncompleteLength == LengthFrame)
                    {
                        ctx.IncompleteMessage = ctx.Alloc.Buffer(ctx.IncompleteHeader);
                    }
                }
                else
                {
                    //buf应该读取剩下的部分
                    int skip = Math.Min(buf.Readables(), ctx.IncompleteHeader - ctx.IncompleteMessage.Readables());

                    //将buf的数据
                    ctx.IncompleteMessage.Write(buf, skip);

                    buf.ReadSkip(skip);
                    i += skip - 1;

                    if (ctx.IncompleteMessage.Readables() == ctx.IncompleteHeader)
                    {
                        output.Add(ctx.IncompleteMessage);

                        ctx.IncompleteMessage = null;
                        ctx.IncompleteLength = 0;
                        ctx.IncompleteHeader = 0;
                    }
                    
                }
            }

        }

        /// <summary>
        /// 资源释放
        /// </summary>
        protected override void Return(IByteBuf message)
        {
            Ensure.IsNotNull(message);

            message.Return();
        }
    }
}
