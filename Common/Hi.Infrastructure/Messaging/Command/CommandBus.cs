using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Messaging.Command
{
    public class CommandBus : IMessageSender<ICommand, CommandResult>
    {

        private ICommandHandleRegisterEntry CommandHandlerRegisterEntry;

        public CommandBus(ICommandHandleRegisterEntry commandHandlerRegisterEntry) {
            CommandHandlerRegisterEntry = commandHandlerRegisterEntry;
        }

        public CommandResult Send(ICommand command)
        {
            var commandType = command.GetType();
            return CommandHandlerRegisterEntry.GetHandler(commandType)?.Invoke(command);
        }

        public Task<CommandResult> SendAsync(ICommand command)
        {
            var commandType = command.GetType();
            return Task.Factory.StartNew(() => CommandHandlerRegisterEntry.GetHandler(commandType)?.Invoke(command));
        }
    }
}
