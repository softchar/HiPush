using Hi.Model.Application.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Messaging.Result
{
    public class UpdateApplicationResult : MessageResult<UpdateApplicationResult, UpdateApplicationCode>
    {
        private UpdateApplicationCode statusCode = UpdateApplicationCode.Success;

        public override UpdateApplicationCode StatusCode
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
