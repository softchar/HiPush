using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Eventloops
{
    public class SchedulerTask : ISchedulerRunable
    {
        Task task;
        long startTicks;
        long deadlineTicks;
        TaskScheduler scheduler;

        public long DeadlineTicks => deadlineTicks;

        public SchedulerTask(TaskScheduler scheduler, Task task, int second)
        {
            this.task = task;
            this.startTicks = DateTime.Now.Ticks;
            this.deadlineTicks = DateTime.Now.Ticks + TimeSpan.FromSeconds(second).Ticks;
            this.scheduler = scheduler;
        }

        public void Run()
        {

        }

        public int CompareTo(ISchedulerRunable other)
        {
            return this.CompareTo(other);
        }

        public bool IsDead()
        {
            return true;
        }

        
    }
}
