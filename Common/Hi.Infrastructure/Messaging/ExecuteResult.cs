using Hi.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Messaging
{
    public class ExecuteResult
    {
        private bool isCompleted = true;
        private ReturnValue values = null;

        public ExecuteResult()
        {
            values = new ReturnValue();
        }

        /// <summary>
        /// 提交/请求状态是否成功
        /// </summary>
        public bool IsCompleted
        {
            get { return isCompleted; }
            set { isCompleted = value; }
        }

        public ReturnValue Values
        {
            get { return values; }
            set { values = value; }
        }
    }
}
