using Hi.Infrastructure.Domain;
using Hi.Infrastructure.UnitOfWork;
using Hi.Model.Application;
using Hi.Model.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Server.Interface;

namespace Hi.Server.Implement
{
    using Hi.Model.Devices.BusinessRules;
    using Messaging.Commanding;
    using Interface;
    using Messaging.Request;
    using Messaging.Result;

    public class DeviceServer : AppServerBase<Device>, IDeviceServer
    {
        private IDeviceRepository _deviceRepository;
        private IApplicationRepository _applicationRepository; 

        public DeviceServer(IDeviceRepository deviceRepository, IApplicationRepository applicationRepository) {
            _deviceRepository = deviceRepository;
            _applicationRepository = applicationRepository;
        }

        /// <summary>
        /// 注册设备
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public RegisterDevicesResult RegisterDevice(RegisterDeviceRequest request)
        {
            var result = new RegisterDevicesResult();

            var device = CreateDevice(request);

            return RegisterDevice(device);
        }

        /// <summary>
        /// 注册设备
        /// </summary>
        /// <param name="device"></param>
        /// <returns>这个方法的存在是为了方便单元测试</returns>
        public RegisterDevicesResult RegisterDevice(Device device) {
            var result = new RegisterDevicesResult();
            try
            {
                Validate(device);
                AppIdIsInvalid(device.AppId);
            }
            catch (BusinessRuleException excep)
            {
                //return result.Create(excep);
            }

            _deviceRepository.Register(device);

            return result;
        }

        public virtual Device CreateDevice(RegisterDeviceRequest request) {
            var result = new Device() { AppId = request.AppId, DeviceToken = request.DeviceToken, DeviceType = request.DeviceType };
            return result;
        }

        public virtual void AppIdIsInvalid(Guid appId) {
            bool ishad = _applicationRepository.AppIdIsInvalid(appId);
            if (ishad) {
                var retValue = new ReturnValue();
                retValue.Set((byte)RegisterDeviceCode.AppIdIsInvalid, DeviceBusinessRule.AppIdIsInvalid);
                throw BusinessRuleException.Create(retValue);
            }
        }

        public ReturnValue RegisterDevice(RegisterDeviceCommand command)
        {
            throw new NotImplementedException();
        }

        public ReturnValue RemoveDeviceByAppId(Guid appId)
        {
            throw new NotImplementedException();
        }
    }
}
