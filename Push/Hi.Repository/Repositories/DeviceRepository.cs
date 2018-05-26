using Hi.Model.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Infrastructure.UnitOfWork;
using Hi.Infrastructure.Querying;
using System.Data.Entity.Core.Objects;
using Hi.Repository.DataContextStorage;


namespace Hi.Repository.Repositories
{
    public class DeviceRepository : Repository<Device, Guid>, IDeviceRepository
    {
        public DeviceRepository(IUnitOfWork uow) : base(uow)
        {
        }

        public Device Register(Device device)
        {
            var application = DataContextFactory.GetDataContext().Application.Where(app => app.AppId == device.AppId && app.IsRemoved == false).FirstOrDefault();
            if (application == null) {
                //app不存在
            }
            DataContextFactory.GetDataContext().Device.Add(device);
            _uow.Commit();
            return device;
        }
    }
}
