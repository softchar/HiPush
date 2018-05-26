using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Eventloops
{
    public class HiTaskScheduler : TaskScheduler
    {
        bool started;
        public SingleThreadEventloop loop;

        public HiTaskScheduler(SingleThreadEventloop loop)
        {
            this.loop = loop;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            throw new NotImplementedException();
        }

        protected override void QueueTask(Task task)
        {
            if (started)
            {
                loop.Execute(new TaskRunnable(this, task));
            }
            else
            {
                this.TryExecuteTask(task);
                started = true;
            }
            
        }

        /// <summary>
        /// 这个方法在出现等待任务的时候才会被执行
        /// 
        /// 返回值
        /// False:表示Task的任务还没有完成,继续等待
        /// True:表示Task的任务已经完成,如果Task.IsCompleted=false,但是返回了True,那么将会抛出异常TaskSchedulerException
        /// </summary>
        /// <param name="task"></param>
        /// <param name="taskWasPreviouslyQueued"></param>
        /// <returns></returns>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (taskWasPreviouslyQueued || !loop.InEventloop)
            {
                return false;
            } 

            return TryExecuteTask(task);
        }

        /// <summary>
        /// 因为TryExecuteTask是受保护的方法(protected描述),只能由其子类去调用
        /// 所以将TaskRunnable设置为HiTaskScheduler的内部类
        /// </summary>
        public class TaskRunnable : ISchedulerRunnable
        {
            Task task;
            HiTaskScheduler scheduler;

            public TaskRunnable(HiTaskScheduler scheduler, Task task)
            {
                this.task = task;
                this.scheduler = scheduler;
            }

            public void Run()
            {
                scheduler.TryExecuteTask(task);
            }
        }

    }

    
}
