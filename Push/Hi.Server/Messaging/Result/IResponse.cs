using Hi.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiPush.Server.Messaging.Response
{
    public interface IResponse<T>
    {
        T Create(ReturnValue retValue);
    }
}
