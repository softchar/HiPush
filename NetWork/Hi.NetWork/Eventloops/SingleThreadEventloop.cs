using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Hi.NetWork.Buffer;
using Hi.NetWork.Socketing.Channels;

namespace Hi.NetWork.Eventloops
{

    /************************************************************************/
    /* Eventloop
     * 每次从任务队列中取出第一个任务；
     * 判断当前时间是否大于运行的时间，大于则运行，小于则等待；
     * 每次只运行50ms，在50ms内如果则行任务数大于等于64，则进行下一次执行
    /************************************************************************/
    public class SingleThreadEventloop : IEventloop
    {
        //任务队列
        private ConcurrentQueue<IRunnable> taskQueue;

        //定时任务队列（优先级队列）
        private PriorityQueue<ISchedulerRunable> schedulerQueue;

        private Thread thread;
        int spinTaskCounter = 64;

        ManualResetEventSlim emptyEvent = new ManualResetEventSlim(false);

        IByteBufAllocator alloc;

        /// <summary>
        /// 当前线程是否是当前Eventloop指定的线程
        /// </summary>
        public bool InEventloop
        {
            get
            {
                return this.thread == Thread.CurrentThread;
            }
        }

        /// <summary>
        /// 内存池
        /// </summary>
        public IByteBufAllocator Alloc
        {
            get
            {
                return alloc;
            }
        }

        public SingleThreadEventloop()
        {
            this.alloc = DefaultByteBufAllocator.Default;
            this.taskQueue = new ConcurrentQueue<IRunnable>();
            this.schedulerQueue = new PriorityQueue<ISchedulerRunable>();

            thread = new Thread(loop) { IsBackground = true };
            thread.Start();
        }

        /// <summary>
        /// 将IRunable放入任务队列
        /// </summary>
        /// <param name="runable"></param>
        public void Execute(IRunnable runable)
        {
            taskQueue.Enqueue(runable);

            if (!InEventloop)
            {
                emptyEvent.Set();
            }
        }

        /// <summary>
        /// 将ISchedulerRunable放入任务队列
        /// </summary>
        /// <param name="task"></param>
        public void Execute(ISchedulerRunable task)
        {
            Execute(new ActionTask(() => { schedulerQueue.Enqueue(task); }));
        }

        public void Register(IChannel channel)
        {
             channel.Invoker.Register(this);
        }

        private void loop()
        {
            while (true)
            {
                PollSchedulerTask();

                var task = PollTask();

                if (task != null)
                {
                    task.Run();
                }
            }
        }

        /// <summary>
        /// 从任务队列中轮询任务
        /// </summary>
        /// <returns></returns>
        private IRunnable PollTask()
        {
            IRunnable task;

            if (!taskQueue.TryDequeue(out task))
            {
                emptyEvent.Reset();
                if (!taskQueue.TryDequeue(out task))
                {
                    emptyEvent.Wait();
                    taskQueue.TryDequeue(out task);
                }
            }

            return task;
        }

        /// <summary>
        /// 将已超时的SchedulerTask加入到执行队列
        /// </summary>
        private void PollSchedulerTask()
        {
            ISchedulerRunable task;
            while ((task = schedulerQueue.Dequeue()) != null && task.IsDead())
            {
                taskQueue.Enqueue(task);
            }
        }

        public void SetAlloc(IByteBufAllocator bufAlloc)
        {
            this.alloc = bufAlloc;
        }
    }
}

