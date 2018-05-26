using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Dto
{
    public class Results<T>
    {

        private bool state = true;
        /// <summary>
        /// 提交/请求状态是否成功
        /// </summary>
        public bool State
        {
            get { return state; }
            set { state = value; }
        }

        /// <summary>
        /// 数据集
        /// </summary>
        public virtual List<T> Data { get; set; }

    }
}
