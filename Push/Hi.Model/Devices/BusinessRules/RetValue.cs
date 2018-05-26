using Hi.Infrastructure.Domain;
using Hi.Infrastructure.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Devices.BusinessRules
{
    public class RegisterDeviceRetValue : ReturnValue {

    }

    public enum RegisterDeviceCode : byte
    {
        Success = 0,                 //成功
        Business = 1,                //违反业务规则错误(eg:比如转账中账户没有钱这些属于违反业务规则)
        AppIdIsInvalid = 2,          //AppId无效       
    }


    /// <summary>
    /// 业务规则
    /// </summary>
    public static class DeviceBusinessRule 
    {
        public static string appId = PropertyNameHelper.ResolvePropertyName<Hi.Model.Application.Application>(app => app.AppId);
        public static string appName = PropertyNameHelper.ResolvePropertyName<Hi.Model.Application.Application>(app => app.AppName);
        public static string deviceToken = PropertyNameHelper.ResolvePropertyName<Device>(d => d.DeviceToken);

        public static readonly BusinessRule DeviceTokenIsNull = new BusinessRule(deviceToken, deviceToken + "不能为空");
        public static readonly BusinessRule AppIdIsEmpty = new BusinessRule(appId, appId + "不能为空");
        public static readonly BusinessRule AppIdIsInvalid = new BusinessRule(appId, appId + "无效");
    } 
} 
