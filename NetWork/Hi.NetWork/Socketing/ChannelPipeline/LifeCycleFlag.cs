using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing.ChannelPipeline
{
    /// <summary>
    /// 生命周期事件枚举
    /// </summary>
    [Flags]
    public enum LifeCycleFlag
    {
        //channal注册事件
        OnChannelRegister = 1 << 0,

        //channel激活
        OnChannelActive = 1 << 1,

        //channel读取
        OnChannelRead = 1 << 2,

        //channel写入
        OnChannelWrite = 1 << 3,

        //channel关闭
        OnChannelClose = 1 << 4,

        //channel终结
        OnChannelFinally = 1 << 5,

        //channel异常
        OnChannelException = 1 << 6,

        //channel写入
        WriteAsync = 1 << 7,

        //channel绑定
        BindAsync = 1 << 8,

        //连接
        ConnectAsync = 1 << 9,

        //入栈: 由系统内部触发的事件
        InBound = OnChannelRead | OnChannelClose | OnChannelException | OnChannelActive | OnChannelRegister | OnChannelFinally,

        //出栈: 由用户主动触发的事件
        OutBound = OnChannelWrite | WriteAsync | BindAsync | ConnectAsync,

        
    }
}
