using Hi.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Pushing
{
    using Messaging;
    using Messaging.Enum;
    using Devices;
    
    public partial class PushMessage : Entity, IAggregateRoot
    {
        public System.Guid AppId { get; set; }
        public System.Guid MessageId { get; set; }
        public System.Guid DeviceId { get; set; }
        public System.Guid DeviceToken { get; set; }
        public MessageDeviceType DeviceType { get; set; }

        public Message Message { get; set; }
    }
}
