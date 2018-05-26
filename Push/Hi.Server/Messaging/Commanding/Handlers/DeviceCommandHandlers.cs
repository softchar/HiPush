using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Infrastructure.Messaging.Command;
using Hi.Server.Interface;

namespace Hi.Server.Messaging.Commanding.Handlers
{
    public class DeviceCommandHandlers :
        ICommandHandler<RegisterDeviceCommand>
    {

        private IDeviceServer deviceServer;

        public DeviceCommandHandlers(IDeviceServer deviceServer)
        {
            this.deviceServer = deviceServer;
        }

        public CommandResult Handle(RegisterDeviceCommand command)
        {
            var retValue = deviceServer.RegisterDevice(command);

            return new CommandResult() { Values = retValue }; 
        }
    }
}
