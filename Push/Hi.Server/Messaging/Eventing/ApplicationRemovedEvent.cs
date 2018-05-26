using Hi.Infrastructure.Messaging.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Messaging.Eventing
{
    public class ApplicationRemovedEvent : IDomainEvent
    {
        public Guid AppId { get; set; }
    }
}
