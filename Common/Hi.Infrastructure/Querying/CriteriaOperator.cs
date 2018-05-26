using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Querying
{
    /// <summary>
    /// 条件选项
    /// </summary>
    public enum CriteriaOperator
    {
        Equal,                  //等于
        LesserThanOrEqual,      //小于或等于
        GreaterOrEqual,         //大于或等于
        NotEqual                //不等于

        //NotApplicable//不应用
    }
}
