using Hi.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Devices
{
    using BusinessRules;
    using Enum;

    public class Device : Entity, IAggregateRoot
    {
        public System.Guid AppId { get; set; } 
        public System.Guid DeviceToken { get; set; } 
        public DeviceType DeviceType { get; set; }

        protected override void Validate()
        {
            if (AppId == Guid.Empty)
            {
                AddBrokenRule(DeviceBusinessRule.AppIdIsEmpty);
            }

            if (DeviceToken == Guid.Empty)
            {
                AddBrokenRule(DeviceBusinessRule.DeviceTokenIsNull);
            }
        }
    }
}
