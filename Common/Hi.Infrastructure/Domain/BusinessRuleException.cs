using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Domain {
    public class BusinessRuleException : Exception
    {

        /// <summary>
        /// 编号
        /// </summary>
        public byte Code
        {
            get
            {
                if (ReturnValue == null)
                    return 0;
                return ReturnValue.Code;
            }
        }


        /// <summary>
        /// 业务规则
        /// </summary>
        public List<BusinessRule> BusinessRules
        {
            get
            {
                if (ReturnValue == null)
                    return null;
                return ReturnValue.BrokenRule;
            }
        }


        public ReturnValue ReturnValue { get; private set; }


        public BusinessRuleException() { }

        private BusinessRuleException(string message, ReturnValue retValue) : base(message)
        {
            ReturnValue = retValue;
        }


        public static BusinessRuleException Create(ReturnValue retValue) {
            string errorMessage = retValue.GetBrokenRuleString();
            return new BusinessRuleException(errorMessage, retValue);
        }
    }
}
