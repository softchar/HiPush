using Hi.Infrastructure.Domain;
using Hi.Model;
using Hi.Model.Application.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Messaging.Result
{
    public class CreateApplicationResult : MessageResult<CreateApplicationResult, CreateApplicationCode>
    {
        private CreateApplicationCode statusCode = CreateApplicationCode.Success;
        
        public override CreateApplicationCode StatusCode {
            get {
                return statusCode;
            }
            set {
                statusCode = value;
            }
        }
    }
}
