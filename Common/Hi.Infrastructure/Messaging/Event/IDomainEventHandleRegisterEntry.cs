using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Messaging.Event
{
    public interface IDomainEventHandleRegisterEntry : IHandleRegisterEntry
    {
        Dictionary<Type, List<Action<IDomainEvent>>> Handlers { get; }
        void RegisterHandler<T>(IDomainEventHandler<T> commandHandler) where T : IDomainEvent;
    }

    public class DomainEventHandleRegisterEntry : HandleRegisterEntryBase, IDomainEventHandleRegisterEntry
    {
        public override Type HandlerType() { return typeof(IDomainEventHandler<>); }
        private Dictionary<Type, List<Action<IDomainEvent>>> handlers = new Dictionary<Type, List<Action<IDomainEvent>>>();

        public Dictionary<Type, List<Action<IDomainEvent>>> Handlers { get { return handlers; } }

        public void RegisterHandler<T>(IDomainEventHandler<T> commandHandler) where T : IDomainEvent
        {

            var t = typeof(T);

            Action<IDomainEvent> act = evnt => commandHandler.Handle((T)evnt);

            List<Action<IDomainEvent>> hds;
            if (handlers.TryGetValue(t, out hds))
            {
                hds.Add(act);
            }
            else
            {
                hds = new List<Action<IDomainEvent>>() { act };
                handlers[t] = hds;
            }
        }
    }
}
