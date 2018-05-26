using Hi.Infrastructure.Domain;
using Hi.Infrastructure.Querying;
using Hi.Model.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Pushing.BusinessRules
{
    public enum CreateMessageDeviceCode : byte
    {
        Success = 0,            //成功
        Business = 1,           //违反业务规则错误(eg:比如转账中账户没有钱这些属于违反业务规则)
    }

    public static class MessageDeviceBusinessRule
    {

    }

    public class CreateMessageDeviceRetValue : ReturnValue
    {

    }
}
