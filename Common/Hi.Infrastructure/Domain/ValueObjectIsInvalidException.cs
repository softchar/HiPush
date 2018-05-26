using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Domain
{
    /// <summary>
    /// 值对象业务规则验证不通过异常
    /// </summary>
    public class ValueObjectIsInvalidException : Exception
    {
        public ValueObjectIsInvalidException(string message) : base(message) {

        }
    }
}
