using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Querying
{
    public static class PropertyNameHelper
    {
        /// <summary>
        /// 获得属性名：eg ResolvePropertyName(o => o.Id), 获得字符串Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static string ResolvePropertyName<T>(Expression<Func<T, object>> expression) {
            var expr = expression.Body as MemberExpression;
            if (expr == null) {
                var u = expression.Body as UnaryExpression;
                expr = u.Operand as MemberExpression;
            }
            return expr.ToString().Substring(expr.ToString().IndexOf(".") + 1);
        }
    }
}
