using Hi.Infrastructure.Domain;
using Hi.Model.Pushing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Infrastructure.Querying;
using System.Linq.Expressions;
using Hi.Infrastructure.UnitOfWork;
using System.Data.Entity.Core.Objects;
using Hi.Model.Pushing.Enum;
using Hi.Repository.DataContextStorage;
using Hi.Model.Messaging;
using System.Data.SqlClient;
using Hi.Model.Devices;

namespace Hi.Repository.Repositories
{
    public class PushMessageRepository : Repository<PushMessage, Guid>, IPushMessageRepository
    {

        public PushMessageRepository(IUnitOfWork uow) : base(uow) { }

        public List<PushMessage> GetPrePushMessages(PushMessageStatus status, int size)
        {
            string sql = @"select P.*, m.DeviceType ,b.Title, b.Content from [Message] as m
                            inner join MessageBody as b on m.BodyToken = b.Token
                            inner join PushMessage as p on p.MessageId = m.Token where p.[Status] = @status ";
            using (var db = DataContextFactory.GetDataContext())
            {
                //var result = db.Database.SqlQuery<PushMessage>(sql, new { status = status }).Take(size);
                var result = db.Database.SqlQuery<Device>("select * from Device");
                var r = result.ToList();
                return null;

                /*
                var pushResult = db.PushMessage.Join(db.MessageBody, pm => pm.MessageId, m => m.Token, (pm, m) => new { Message = m, PushMessage = pm })
                    .Select((a, i) => new PushMessage()
                    {
                        Message = new Message() { AppId = a.PushMessage.AppId, Token = a.PushMessage.MessageId, }
                    });
                */
            } 
        }

        public PushMessage Register(PushMessage pushMessage)
        {
            Add(pushMessage);
            _uow.Commit();
            return pushMessage;
        }
    }
}
