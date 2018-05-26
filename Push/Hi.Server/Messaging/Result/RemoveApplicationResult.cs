using Hi.Model.Application.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Messaging.Result
{
    public class RemoveApplicationResult : MessageResult<RemoveApplicationResult, RemoveApplicationCode> {
        private RemoveApplicationCode statusCode = RemoveApplicationCode.Success;

        public override RemoveApplicationCode StatusCode
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
