using Hi.Infrastructure.Domain;
using Hi.Model.Messaging.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Messaging {
    public class MessageServer {

        private IMessageRepository _messageRepository;
         
        public MessageServer(IMessageRepository messageRepository) {
            _messageRepository = messageRepository;
        }

        public CreateMessageRetValue CreateMessage(Message message) {

            //先创建一个返回值对象
            var retValue = new CreateMessageRetValue();

            //创建对象之前的业务验证
            createValidate(message);

            _messageRepository.Add(message);

            //设置成功状态
            retValue.Set((byte)CreateMessageCode.Success);

            return retValue;

        }
        /// <summary>
        /// 创建之前的业务验证
        /// </summary>
        private void createValidate(Message message) {

            var retValue = new CreateMessageRetValue();

            var brokenRules = message.GetBrokenRules().ToList();

            if (brokenRules.Count() > 0) {
                retValue.Set((byte)CreateMessageCode.Business, brokenRules);
                throw new BusinessRuleException().Create(retValue);
            }
        }
    }
}
