using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model
{
    /// <summary>
    /// 通用返回值
    /// </summary>
    public enum GeneralRetValue : byte
    {
        Success = 0,            //成功
        Business = 1,           //违反业务规则错误(eg:比如转账中账户没有钱这些属于违反业务规则)
    }
}
