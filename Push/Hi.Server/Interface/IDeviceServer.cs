
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Infrastructure.Domain;

namespace Hi.Server.Interface
{
    using Hi.Server.Messaging.Request;
    using Hi.Server.Messaging.Result;
    using Messaging.Commanding;

    public interface IDeviceServer
    {
        /// <summary>
        /// 注册设备
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        ReturnValue RegisterDevice(RegisterDeviceCommand command);
        ReturnValue RemoveDeviceByAppId(Guid appId);
    }
}
