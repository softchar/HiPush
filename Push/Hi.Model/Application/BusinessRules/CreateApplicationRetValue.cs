using Hi.Infrastructure.Domain;
using Hi.Infrastructure.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Application.BusinessRules
{
    /// <summary>
    /// 返回值
    /// </summary>
    public class CreateApplicationRetValue : ReturnValue {

    }

    /// <summary>
    /// 返回值状态
    /// </summary>
    public enum CreateApplicationCode : byte
    {
        Success = 0,                //成功
        Business = 1,               //应用程序已存在
        AppIdHad = 101,             //违反业务规则错误(eg:比如转账中账户没有钱这些属于违反业务规则)
        AppNameIsNull = 102,        //appName为空
    }

    /// <summary>
    /// 业务规则
    /// </summary>
    public static class ApplicationBusinessRule
    {
        public static readonly BusinessRule AppIdIsNull = new BusinessRule(PropertyNameHelper.ResolvePropertyName<Application>(app => app.AppId), PropertyNameHelper.ResolvePropertyName<Application>(app => app.AppId) + "不能为空");
        public static readonly BusinessRule AppIdIsHad = new BusinessRule(PropertyNameHelper.ResolvePropertyName<Application>(app => app.AppId), PropertyNameHelper.ResolvePropertyName<Application>(app => app.AppId) + "已存在");
        public static readonly BusinessRule AppNameIsNull = new BusinessRule(PropertyNameHelper.ResolvePropertyName<Application>(app => app.AppName), PropertyNameHelper.ResolvePropertyName<Application>(app => app.AppName) + "不能为空");
        public static readonly BusinessRule AppNameIsHad = new BusinessRule(PropertyNameHelper.ResolvePropertyName<Application>(app => app.AppName), PropertyNameHelper.ResolvePropertyName<Application>(app => app.AppName) + "已存在");
    } 
}
