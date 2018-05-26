using Hi.Model.Messaging.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Messaging.Result
{
    public class CreateMessageResult : MessageResult<CreateMessageResult, CreateMessageCode>
    {
        private CreateMessageCode statusCode = CreateMessageCode.Success;
        public override CreateMessageCode StatusCode
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
