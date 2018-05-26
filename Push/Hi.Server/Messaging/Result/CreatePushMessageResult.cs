using Hi.Model.Pushing.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Messaging.Result
{
    public class CreatePushMessageResult : MessageResult<CreatePushMessageResult, CreateMessageDeviceCode>
    {
        private CreateMessageDeviceCode statusCode = CreateMessageDeviceCode.Success;
        public override CreateMessageDeviceCode StatusCode
        {
            get
            {
                return statusCode;
            }
            set
            {
                statusCode = value;
            }
        }
    }
}
