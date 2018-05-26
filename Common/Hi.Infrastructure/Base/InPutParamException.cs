using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Base
{
    /// <summary>
    /// 输入参数异常
    /// </summary>
    public class InPutParamException : Exception
    {
        public Type InputParamType;
        public object InputParamValue;
        public string Reason;
        public InPutParamException(string message)
            : base(message)
        {

        } 
    }
}
