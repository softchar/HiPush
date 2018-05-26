using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Messaging.Command
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        CommandResult Handle(TCommand command);
    }
}
