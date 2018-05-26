using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing {

    /************************************************************************/
    /* 为Channel提供对外服务,作用类似于HttpContext,                           */
    /************************************************************************/
    public interface IChannel {

        void Send(byte[] data);

        /// <summary>
        /// 初始化通道
        /// </summary>
        /// <param name="socket"></param>
        void OnAccept(Socket socket);



        /// <summary>
        /// 获得远程的节点信息
        /// </summary>
        /// <returns></returns>
        string GetRemoteNode();

        //IChannelPipeline Pipeline();


    }
}
