
using Hi.Infrastructure.Domain;
using Hi.Infrastructure.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Messaging.BusinessRules {

    public enum CreateMessageCode : byte {
        Success = 0,            //成功
        Business = 1,           //违反业务规则错误(eg:比如转账中账户没有钱这些属于违反业务规则)
        AppIdIsInvalid = 2,         //AppId无效
    }

    public static class MessageBusinessRule
    {
        public static readonly BusinessRule BodyIsNull = new BusinessRule(PropertyNameHelper.ResolvePropertyName<Message>(message => message.Body), "Body不能为空");
        public static readonly BusinessRule TitleIsNullOrEmpty = new BusinessRule(PropertyNameHelper.ResolvePropertyName<MessageBody>(body => body.Title), "Title不能为空或长度大于50");
        public static readonly BusinessRule ContentIsNullOrEmpty = new BusinessRule(PropertyNameHelper.ResolvePropertyName<MessageBody>(body => body.Content), "Content不能为空或长度大于200");
        public static readonly BusinessRule AppIdIsInvalid = new BusinessRule(PropertyNameHelper.ResolvePropertyName<Hi.Model.Application.Application>(app => app.AppId), "AppId无效");
    }

    public class CreateMessageRetValue : ReturnValue {

    }
}
