using Hi.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Pushing
{
    using Messaging;
    using Devices;

    public class PushMessagePackage : Entity
    {
        public Message Message { get; private set; }
        public List<Device> Devices { get; private set; }
    }
}
