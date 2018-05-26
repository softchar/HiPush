using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Domain {

    /// <summary>
    /// 业务返回值
    /// </summary>
    public class ReturnValue {

        /// <summary>
        /// 返回值编号
        /// </summary>
        public byte Code { get; private set; }

        /// <summary>
        /// 业务规则
        /// </summary>
        public List<BusinessRule> BrokenRule { get; private set; }

        public ReturnValue() {
            Code = 0;
        }

        public ReturnValue(BusinessRuleException excep) {
            Code = excep.Code;
            BrokenRule = excep.BusinessRules;
        }

        public void Set(byte code) {
            Code = code;
        }
        public void Set(byte code, List<BusinessRule> brokenRules) {
            Code = code;
            if (BrokenRule == null) BrokenRule = new List<BusinessRule>();
            BrokenRule.AddRange(brokenRules);
        }
        public void Set(byte code, BusinessRule brokenRule) {
            Code = code;
            if (BrokenRule == null) BrokenRule = new List<BusinessRule>();
            BrokenRule.Add(brokenRule);
        }

        public string GetBrokenRuleString() {
            StringBuilder strBuilder = new StringBuilder();
            if (BrokenRule == null)
                return string.Empty;

            foreach (var item in BrokenRule) {
                strBuilder.Append(item.Property + "," + item.Rule);
            }
            return strBuilder.ToString();
        }

        public bool IsSuccess {
            get {
                return BrokenRule != null && BrokenRule.Count > 0;
            }
        }
    }

}
