using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Pushing
{
    using Messaging;
    using Devices;

    public class MessageDeviceServer
    {
        private IMessageRepository _messageRepository;
        private IMessageDeviceRepository _messageDeviceRepository;

        public MessageDeviceServer(IMessageRepository messageRepository, IMessageDeviceRepository messageDeviceRepository)
        {
            _messageRepository = messageRepository;
            _messageDeviceRepository = messageDeviceRepository;
        }

        /// <summary>
        /// 为设备添加一条即将开始推送的消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="device"></param>
        public void CreatePushMessageForDevice(Message message, Device device) {
            var messageDevice = new MessageDevice() { DeviceId = device.Token, DeviceToken = device.DeviceToken, DeviceType = message.DeviceType, MessageId = message.Token, AppId = device.AppId };
            _messageDeviceRepository.Add(messageDevice);
        }

        /// <summary>
        /// 为设备添加一条即将开始推送的消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="device"></param>
        public void CreatePushMessageForDevice(PushMessage pushMessage) {
            foreach (var device in pushMessage.Devices) {
                CreatePushMessageForDevice(pushMessage.Message, device);
            }
        }
    }
}
