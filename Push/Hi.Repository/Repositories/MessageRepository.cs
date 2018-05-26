
using Hi.Model.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Infrastructure.UnitOfWork;
using Hi.Infrastructure.Querying;
using System.Data.Entity.Core.Objects;
using Hi.Repository.DataContextStorage;
using Hi.Infrastructure.Domain;

namespace Hi.Repository.Repositories
{
    
    public class MessageRepository : Repository<Message, Guid>, IMessageRepository
    {
        

        public MessageRepository(IUnitOfWork uow) : base(uow) {
        }

        public override ObjectQuery<Message> TranslateIntoObjectQueryFrom(Query query)
        {
            throw new NotImplementedException();
        }

        
        /// <summary>
        /// 创建消息
        /// </summary>
        /// <param name="message"></param>
        public Message Register(Message message) {
            Add(message);
            AddBody(message.Body);
            _uow.Commit();
            return message;
        }

        private void AddBody(MessageBody body) {
            DataContextFactory.GetDataContext().MessageBody.Add(body);
        }
    }
    
}
