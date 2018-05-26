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

    public class DeleteAppcationRetValue : ReturnValue { }

    /// <summary>
    /// 返回值状态
    /// </summary>
    public enum CreateApplicationCode : byte
    {
        Success = 0,                //成功
        Business = 1,               //应用程序已存在
        AppIdHad = 101,             //违反业务规则错误(eg:比如转账中账户没有钱这些属于违反业务规则)
        AppNameIsNull = 102,        //appName为空
        AppNameIsHad = 103,         //appName已存在
    }

    public enum RemoveApplicationCode : byte
    {
        Success = 0,
        Business = 1,

        //前100位空置

    }

    public enum UpdateApplicationCode : byte {
        Success = 0,
        Business = 1,
        
    }



    /// <summary>
    /// 业务规则
    /// </summary>
    public static class ApplicationBusinessRule
    {
        private static string appId = PropertyNameHelper.ResolvePropertyName<Application>(app => app.AppId);
        private static string appName = PropertyNameHelper.ResolvePropertyName<Application>(app => app.AppName);

        public static readonly BusinessRule AppIdIsNull = new BusinessRule(appId, appId + "不能为空");
        public static readonly BusinessRule AppIdIsHad = new BusinessRule(appId, appId + "已存在");
        public static readonly BusinessRule AppNameIsNull = new BusinessRule(appName, appName + "不能为空");
        public static readonly BusinessRule AppNameIsHad = new BusinessRule(appName, appName + "已存在");
    } 
}
