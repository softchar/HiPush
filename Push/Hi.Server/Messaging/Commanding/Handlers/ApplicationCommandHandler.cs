using Hi.Infrastructure.Messaging.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Model.Application;
using Hi.Server.Interface;

namespace Hi.Server.Messaging.Commanding.Handlers
{
    public class ApplicationCommandHandler : 
        ICommandHandler<CreateApplicationCommand>,
        ICommandHandler<UpdateApplicationCommand>,
        ICommandHandler<RemoveApplicationCommand>
    {

        private IApplicationServer _applicationServer;

        public ApplicationCommandHandler(IApplicationServer applicationServer)
        {
            _applicationServer = applicationServer;
        }

        public CommandResult Handle(RemoveApplicationCommand command)
        {
            var retValue = _applicationServer.Remove(command);

            return new CommandResult() { Values = retValue };
        }

        public CommandResult Handle(UpdateApplicationCommand command)
        {
            var retValue = _applicationServer.Update(command);

            return new CommandResult() { Values = retValue };
        }

        public CommandResult Handle(CreateApplicationCommand command)
        {
            var returnValue = _applicationServer.Create(command);

            return new CommandResult() { Values = returnValue };   
        }
    }
}
