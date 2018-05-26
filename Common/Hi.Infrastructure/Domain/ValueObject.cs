using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Domain
{
    /// <summary>
    /// 值对象抽象类,所有的值对象的基类
    /// </summary>
    public abstract class ValueObject
    {
        private List<BusinessRule> _brokenRules = new List<BusinessRule>();

        public ValueObject() { }

        protected abstract void Validate();

        /// <summary>
        /// 如果值对象的业务规则验证不通过,那么抛出异常
        /// </summary>
        public void ThrowExceptionIfInvalid() {
            _brokenRules.Clear();
            Validate();
            if (_brokenRules.Count() > 0) {
                StringBuilder issues = new StringBuilder();
                foreach (var item in _brokenRules)
                    issues.AppendLine(item.Rule);
                throw new ValueObjectIsInvalidException(issues.ToString());
            }
        }

        protected void AddBrokenRule(BusinessRule businessRule) {
            _brokenRules.Add(businessRule);
        }
    }
}
