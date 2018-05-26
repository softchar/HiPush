using Hi.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Devices
{
    public interface IDeviceRepository 
    {
        Device Register(Device device);
    }
}
