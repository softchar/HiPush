using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Pushing.Enum
{
    /// <summary>
    /// 消息推送的状态
    /// </summary>
    public enum PushMessageStatus : byte
    {
        RREPUSH = 0,//准备推送
        PUSHING = 1,//正在处理
        FINISH = 2,//完成 
    }
}
