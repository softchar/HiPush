using Hi.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Messaging
{
    public class MessageEntryEvent : Entity
    {

        public System.Guid AppId { get; set; }
        public System.Guid MessageId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
