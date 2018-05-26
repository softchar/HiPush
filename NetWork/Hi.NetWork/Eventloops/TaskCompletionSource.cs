using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Eventloops
{
    /// <summary>
    /// 0: 失败
    /// 1: 成功
    /// 3: 异常（错误）
    /// </summary>
    public class TaskCompletionSource : TaskCompletionSource<int>
    {
        public void Success()
        {
            this.SetResult(1);
        }
        public void Fail()
        {
            this.SetResult(0);
        }

        public void Exception(Exception excep)
        {
            this.TrySetException(excep);
        }

    }
}
