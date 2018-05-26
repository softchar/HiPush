using Hi.Infrastructure.Messaging.Event;
using Hi.Server.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Messaging.Eventing.Handlers
{
    public class DeviceEventHandler :
        IDomainEventHandler<ApplicationCreatedEvent>
    {

        IDeviceServer deviceServer;

        public DeviceEventHandler(IDeviceServer deviceServer)
        {
            this.deviceServer = deviceServer;
        }

        public void Handle(ApplicationCreatedEvent @event)
        {
            deviceServer.RemoveDeviceByAppId(@event.AppId);
        }
    }
}
