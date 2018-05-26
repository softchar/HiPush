using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Querying
{
    /// <summary>
    /// 排序对象
    /// </summary>
    public class OrderByClause
    {
        /// <summary>
        /// 排序属性
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 顺序
        /// </summary>
        public bool Desc { get; set; }
    }
}
