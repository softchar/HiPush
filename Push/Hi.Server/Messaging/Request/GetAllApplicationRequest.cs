using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Messaging.Request
{
    public class GetAllApplicationRequest : RequestBase
    {
        public string AppName { get; set; }
    }
}
