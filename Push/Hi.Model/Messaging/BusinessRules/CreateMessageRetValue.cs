
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
        MessageBodyIsNull = 1,  //消息实体为空
        Business = 2,           //违反业务规则错误(eg:比如转账中账户没有钱这些属于违反业务规则)
    }

    public static class MessageBusinessRule {
        public static readonly BusinessRule BodyIsNull = new BusinessRule(PropertyNameHelper.ResolvePropertyName<Message>(message => message.Body), "Body不能为空");
    }

    public class CreateMessageRetValue : ReturnValue {

    }
}
