using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing.ChannelPipeline
{
    /// <summary>
    /// Channel边界方向
    /// </summary>
    public enum ChannelBoundFlag : byte
    {
        //入站
        Inbound,

        //出站
        Outbound
    }
}
