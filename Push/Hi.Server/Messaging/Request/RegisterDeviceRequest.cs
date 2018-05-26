using Hi.Model.Devices.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Messaging.Request
{
    public class RegisterDeviceRequest
    {
        public System.Guid AppId { get; set; }
        public System.Guid DeviceToken { get; set; }
        public DeviceType DeviceType { get; set; }
    }
}
