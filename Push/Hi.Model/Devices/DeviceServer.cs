using Hi.Infrastructure.Domain;
using Hi.Infrastructure.Querying;
using Hi.Model.Devices.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Devices
{

    /*
    public class DeviceServer {

        private IDeviceRepository _deviceRepository;

        public DeviceServer(IDeviceRepository deviceRepository) {
            _deviceRepository = deviceRepository;
        }

        /// <summary>
        /// 注册设备
        /// </summary>
        /// <param name="device"></param>
        public ReturnValue Register(Device device)
        {
            var result = new ReturnValue();

            //验证业务规则
            businessRuleValidation(device, result);

            var _devices = _deviceRepository.FindBy(dvis => dvis.AppId == device.AppId && dvis.DeviceToken == device.DeviceToken);
            if (_devices.Count() > 0)
            {
                var _device = _devices.FirstOrDefault();
                _device = device;
                _deviceRepository.Save(_device);
            }
            else
            {
                _deviceRepository.Add(device);
            }

            result.Set((byte)GeneralRetValue.Success);

            return result;
        }

        /// <summary>
        /// 验证业务规则
        /// </summary>
        /// <param name="device"></param>
        /// <param name="result"></param>
        private void businessRuleValidation(Device device, ReturnValue result) {
            //将对象进行业务验证
            var brokenRules = device.GetBrokenRules();
            if (brokenRules.Count() > 0)
            {
                result.Set((byte)RegisterDeviceCode.Business, brokenRules);
                throw new BusinessRuleException().Create(result);
            }
        }
        

        /// <summary>
        /// 批量注册
        /// </summary>
        /// <param name="devices"></param>
        public void RegisterList(List<Device> devices)
        {
            foreach (var device in devices) 
            {
                Register(device);
            }
        }

        public List<Device> GetDevices(Guid appId)
        {
            var result = _deviceRepository.FindBy(device => device.AppId == appId && device.IsRemoved == false);
            return result.ToList();
        }

        public ReturnValue Delete(Device device)
        {
            var result = new ReturnValue();

            //验证业务规则
            businessRuleValidation(device, result);

            //删除device
            _deviceRepository.Remove(device);

            result.Set((byte)GeneralRetValue.Success);

            return result;
        }
    }

    */
}
