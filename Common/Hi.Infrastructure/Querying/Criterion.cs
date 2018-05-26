using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Querying
{
    /// <summary>
    /// 查询条件对象
    /// </summary>
    public class Criterion
    {
        private string _propertyName;
        private object _value;
        private CriteriaOperator _criteriaOperator;

        public Criterion(string propertyName, object value, CriteriaOperator criteriaOperator) {
            _propertyName = propertyName;
            _value = value;
            _criteriaOperator = criteriaOperator;
        }

        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName {
            get { return _propertyName; }
        }
        
        /// <summary>
        /// 属性值
        /// </summary>
        public object Value {
            get { return _value; }
        }

        /// <summary>
        /// 操作
        /// </summary>
        public CriteriaOperator criteriaOperator {
            get { return _criteriaOperator; }
        }

        public static Criterion Create<T>(Expression<Func<T, object>> expression, object value, CriteriaOperator criteriaOperator){
            string propertyName = PropertyNameHelper.ResolvePropertyName<T>(expression);
            var myCriterion = new Criterion(propertyName, value, criteriaOperator);
            return myCriterion;
        }
    }
}
