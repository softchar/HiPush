using Hi.Infrastructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Hi.NetWork.Eventloops
{
    /// <summary>
    /// 普通任务
    /// </summary>
    public class ActionTask : IRunnable
    {
        public Action action;

        public ActionTask(Action action)
        {
            Ensure.IsNotNull(action);

            this.action = action;
        }

        public void Run()
        {
            action();
        } 

    }
}
