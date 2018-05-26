using Hi.Infrastructure.Domain;
using Hi.Infrastructure.UnitOfWork;
using Hi.Model.Application;
using Hi.Model.Messaging;
using Hi.Model.Messaging.BusinessRules;
using Hi.Server.Interface;
using Hi.Server.Messaging.Request;
using Hi.Server.Messaging.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Implement {
    public class MessageServer : AppServerBase<Message>
    {

        private IMessageRepository _messageRepository;
        private IApplicationRepository _applicatinoRepository;

        public MessageServer(IMessageRepository messageRepository, IApplicationRepository applicatinoRepository) {
            _messageRepository = messageRepository;
            _applicatinoRepository = applicatinoRepository;
        } 

        /// <summary>
        /// 新建消息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CreateMessageResult CreateMessage(CreateMessageRequest request) {

            var result = new CreateMessageResult();

            var messageBody = new MessageBody() {
                Title = request.Title,
                Content = request.Content
            };

            var message = new Message() { AppId = request.AppId  };
            message.SetBody(messageBody);

            try
            {
                Validate(message);
                AppIdIsInvalid(request.AppId);
            }
            catch (BusinessRuleException excep)
            {
                // TODO...
                //return result.Create(excep);
            }

            _messageRepository.Register(message);

            return result;
        }

        /// <summary>
        /// 判断AppId是否无效
        /// </summary>
        /// <param name="appId"></param>
        public virtual void AppIdIsInvalid(Guid appId) {
            var result = _applicatinoRepository.AppIdIsInvalid(appId);
            if (result) { 
                var retValue = new ReturnValue();
                retValue.Set((byte)CreateMessageCode.AppIdIsInvalid, MessageBusinessRule.AppIdIsInvalid);
                throw BusinessRuleException.Create(retValue);
            } 
        }
    }
}
