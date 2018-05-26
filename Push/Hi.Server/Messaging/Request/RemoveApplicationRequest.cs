using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Messaging.Request
{
    public class RemoveApplicationRequest : RequestBase
    {
        public Guid AppId { get; set; }
        public Guid Token { get; set; }
    }
}
