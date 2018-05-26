using Hi.NetWork.Buffer;
using Hi.NetWork.Eventloops;
using Hi.NetWork.Socketing.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing.ChannelPipeline
{
    public interface IChannelPipelineExecutor
    {
        
        void fireChannelRegister();

        void fireChannelActive();

        void fireChannelRead(object messasge);

        void fireChannelWrite(object message);

        void fireChannelClose();

        void fireChannelException();

        void fireChannelFinally();

        Task WriteAsync(object message);

        Task BindAsync(EndPoint address);

        Task ConnectAsync(EndPoint remote);
    }
}
