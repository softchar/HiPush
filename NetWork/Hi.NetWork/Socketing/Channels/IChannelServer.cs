using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing.Channels
{
    using System.Net.Sockets;

    public interface IChannelServer
    {
        /// <summary>
        /// 创建一个服务通道通道
        /// </summary>
        void OnAccept(Socket socket);

        IChannel Connection(Socket socket);

        /// <summary>
        /// 设置通道配置文件
        /// </summary>
        /// <param name="config"></param>
        void SetChannelConfig(ChannelConfig config);
    }
}
