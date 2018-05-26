using Hi.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Messaging.Command
{
    public interface ICommandHandleRegisterEntry : IHandleRegisterEntry
    {
        Dictionary<Type, Func<ICommand, CommandResult>> Handlers { get; }
        void RegisterHandler<T>(ICommandHandler<T> commandHandler) where T : ICommand;

        Func<ICommand, CommandResult> GetHandler(Type commandType);
    }

    public class CommandHandleRegisterEntry : HandleRegisterEntryBase, ICommandHandleRegisterEntry
    {
        public override Type HandlerType() { return typeof(ICommandHandler<>); }

        private Dictionary<Type, Func<ICommand, CommandResult>> handlers = new Dictionary<Type, Func<ICommand, CommandResult>>();

        public Dictionary<Type, Func<ICommand, CommandResult>> Handlers { get { return handlers; } }

        public void RegisterHandler<T>(ICommandHandler<T> commandHandler) where T : ICommand
        {

            var t = typeof(T);

            Func<ICommand, CommandResult> act = evnt => commandHandler.Handle((T)evnt);

            if (!handlers.ContainsKey(t))
            {
                handlers.Add(t, act);
            }
            
        }

        public Func<ICommand, CommandResult> GetHandler(Type commandType)
        {

            if (handlers.ContainsKey(commandType)) {
                return handlers[commandType];
            }

            return null;
        }
    }
}
