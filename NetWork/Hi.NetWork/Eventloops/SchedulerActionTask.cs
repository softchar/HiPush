using Hi.Infrastructure.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Eventloops
{

    /// <summary>
    /// 被调度的普通任务,即Action任务
    /// </summary>
    public class SchedulerActionTask : ISchedulerRunable
    {
        private TaskCompletionSource promise;
        private Action<TaskCompletionSource> task;
        private long startTime;
        private long deadline;


        public SchedulerActionTask(Action<TaskCompletionSource> task, TaskCompletionSource taskSource, int second)
        {
            Ensure.IsNotNull(task);

            this.promise = taskSource;
            this.task = task;

            this.startTime = DateTime.Now.Ticks;;
            this.deadline = DateTime.Now.Ticks + TimeSpan.FromSeconds(second).Ticks;
        }

        public long DeadlineTicks
        {
            get
            {
                return deadline;
            }
        }

        public int CompareTo(ISchedulerRunable other)
        {
            return deadline.CompareTo(other.DeadlineTicks);
        }

        public bool IsDead()
        {
            return DateTime.Now.Ticks >= deadline;
        }

        public void Run()
        {
            //TODU
            try
            {
                task(promise);
            }
            catch (Exception excep)
            {
                promise.SetException(excep);
            }
            
        }
    }
}
