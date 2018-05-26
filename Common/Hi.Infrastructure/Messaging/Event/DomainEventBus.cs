using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Messaging.Event
{
    public class DomainEventBus : IMessageSender<IDomainEvent, EventResult>
    {

        private IDomainEventHandleRegisterEntry DomainEventHandleRegisterEntry;

        public DomainEventBus(IDomainEventHandleRegisterEntry domainEventHandleRegisterEntry)
        {
            DomainEventHandleRegisterEntry = domainEventHandleRegisterEntry;
        }


        public EventResult Send(IDomainEvent message)
        {
            var result = new EventResult();

            List<Action<IDomainEvent>> handlers;

            if (!DomainEventHandleRegisterEntry.Handlers.TryGetValue(message.GetType(), out handlers))
            {
                return result;
            }

            try
            {
                handlers.ForEach(handler => handler(message));
            }
            catch (Exception e)
            {
                result.IsCompleted = false;
                return result;
            }

            return result;
        }

        public Task<EventResult> SendAsync(IDomainEvent message)
        {
            var result = new EventResult();

            List<Action<IDomainEvent>> handlers;

            if (!DomainEventHandleRegisterEntry.Handlers.TryGetValue(message.GetType(), out handlers))
            {
                return Task.FromResult(result);
            }

            try
            {
                handlers.ForEach(handler => Task.Factory.StartNew(() => { handler(message); }));
            }
            catch (Exception e)
            {
                result.IsCompleted = false;
                return Task.FromResult(result);
            }

            return Task.FromResult(result);

        }

    }
}
