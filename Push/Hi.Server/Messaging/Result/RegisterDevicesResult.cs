using Hi.Infrastructure.Domain;
using Hi.Model.Application.BusinessRules;
using Hi.Model.Devices.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Messaging.Result
{
    public class RegisterDevicesResult : MessageResult<RegisterDevicesResult, RegisterDeviceCode>
    {
        private RegisterDeviceCode statusCode = RegisterDeviceCode.Success;
        public override RegisterDeviceCode StatusCode
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
