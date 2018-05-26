using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Messaging.Enum
{
    /// <summary>
    /// 消息的发送目标类型
    /// </summary>
    public enum MessageDeviceType : byte
    {
        All,
        Android,
        SignalR,
        IOS
    }
}
