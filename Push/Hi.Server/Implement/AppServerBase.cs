using Hi.Infrastructure.Domain;
using Hi.Infrastructure.Ioc;
using Hi.Infrastructure.UnitOfWork;
using Hi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Implement
{
    
    public class AppServerBase<T> where T : Entity
    {
        protected IRepository<Hi.Model.Application.Application, Guid> Repository;
         
        /// <summary>
        /// 创建App之前的业务验证
        /// </summary>
        protected void Validate(T t)
        {
            var brokenRules = t.GetBrokenRules().ToList();
            if (brokenRules.Count() > 0)
            {
                var retValue = new ReturnValue();
                retValue.Set((byte)GeneralRetValue.Business, brokenRules);
                throw BusinessRuleException.Create(retValue);
            }
        }

        /// <summary>
        /// 创建App之前的业务验证
        /// </summary>
        protected void Validate<TEntity>(TEntity t) where TEntity : Entity
        {
            var brokenRules = t.GetBrokenRules().ToList();
            if (brokenRules.Count() > 0)
            {
                var retValue = new ReturnValue();
                retValue.Set((byte)GeneralRetValue.Business, brokenRules);
                throw BusinessRuleException.Create(retValue);
            }
        }
    }
}
