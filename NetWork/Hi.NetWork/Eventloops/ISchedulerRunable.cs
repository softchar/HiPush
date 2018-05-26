using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Eventloops
{
    public interface ISchedulerRunable : IRunnable, IComparable<ISchedulerRunable>
    {
        /// <summary>
        /// 截止日期
        /// </summary>
        long DeadlineTicks { get; }

        /// <summary>
        /// 是否已经超时
        /// </summary>
        /// <returns></returns>
        bool IsDead();
    }
}
