using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Messaging.Request
{
    public class UpdateApplicationRequest : RequestBase
    {
        public Guid AppId { get; set; }
        public string AppName { get; set; }
    }
}
