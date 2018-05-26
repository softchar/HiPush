using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.NetWork.Socketing.ChannelPipeline;

namespace Hi.NetWork.Client
{
    public class MyChannelHandler2 : ChannelHandler
    {
        public override void OnChannelActive(IChannelHandlerContext context)
        {
            base.OnChannelActive(context);
        }
    }
}
