using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing.Sockets
{

    public class ChannelConfig
    {
        public int SendingBufferSize = 1024 * 4;        //发送缓冲区的大小
        public int ReceivingBufferSize = 1024 * 4;      //接收缓冲区的大小
        public int PenddingMessageCounter = 1024;       //发送时阻塞的最大的消息个数
        public bool AutoReceiving = true;               //是否自动接收数据
    }
}
