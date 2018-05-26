
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Implement
{
    using Hi.Infrastructure.Domain;
    using Hi.Infrastructure.UnitOfWork;
    using Hi.Model.Devices;
    using Hi.Model.Messaging;
    using Hi.Model.Pushing;
    using Hi.Model.Pushing.Enum;
    using Hi.Server.Messaging.Result;

    public class PushMessageServer : AppServerBase<PushMessage>
    {
        private IMessageRepository _messageRepository;
        private IPushMessageRepository _pushMessageRepository;

        public PushMessageServer(IMessageRepository messageRepository, IPushMessageRepository messageDeviceRepository)
        {
            _messageRepository = messageRepository;
            _pushMessageRepository = messageDeviceRepository;
        }

        /// <summary>
        /// 为设备添加一条即将开始推送的消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="device"></param>
        public CreatePushMessageResult CreatePushMessageForDevice(Message message, Device device)
        {
            var result = new CreatePushMessageResult();

            var messageDevice = new PushMessage() { DeviceId = device.Token, DeviceToken = device.DeviceToken, DeviceType = message.DeviceType, MessageId = message.Token, AppId = device.AppId };

            try
            {
                Validate(messageDevice);
            }
            catch (BusinessRuleException excep)
            {
                //return result.Create(excep);
            }

            _pushMessageRepository.Register(messageDevice);

            return result;
        }

        /// <summary>
        /// 获得预推送的消息包
        /// </summary>
        /// <param name="size">包的大小</param>
        /// <returns></returns>
        public List<PushMessagePackage> GetPrePushMessagePack(int size)
        {
            
            var pushMessages = _pushMessageRepository.GetPrePushMessages(PushMessageStatus.RREPUSH, size);
            if (pushMessages == null || pushMessages.Count() <= 0)
                return null;

            return null;
        }

        /// <summary>
        /// 为设备添加一条即将开始推送的消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="device"></param>
        public void CreatePushMessageForDevice(PushMessagePackage pushMessage)
        {
            foreach (var device in pushMessage.Devices)
            {
                CreatePushMessageForDevice(pushMessage.Message, device);
            }
        }
    }
}
