using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Dto
{
    public class PageResult<T> : Results<T>
    {

        /// <summary>
        /// 总数
        /// </summary>
        public virtual int Count { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public virtual int Pages { get; set; }

        public virtual int PageIndex { get; set; }
        public virtual int PageSize { get; set; }

        public virtual int Total { get; set; }
    }
}
