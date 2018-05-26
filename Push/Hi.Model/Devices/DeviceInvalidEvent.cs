using Hi.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Devices
{
    public class DeviceInvalidEvent : Entity
    {
        public System.Guid AppId { get; set; } 
        public System.Guid DeviceId { get; set; } 
        public System.Guid DeviceToken { get; set; } 
    }
}
