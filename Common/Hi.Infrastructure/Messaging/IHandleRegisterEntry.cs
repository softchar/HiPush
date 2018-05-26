using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Messaging
{
    public interface IHandleRegisterEntry
    {
        Type HandlerType();
    }
}
