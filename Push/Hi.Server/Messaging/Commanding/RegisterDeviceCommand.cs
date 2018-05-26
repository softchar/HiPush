using Hi.Infrastructure.Messaging.Command;
using Hi.Model.Devices.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Messaging.Commanding
{
    public class RegisterDeviceCommand : ICommand
    {
        public System.Guid AppId { get; set; }
        public System.Guid DeviceToken { get; set; }
        public DeviceType DeviceType { get; set; }
    }
}
