using Hi.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Infrastructure.Querying;

namespace Hi.Model.Messaging
{
    public interface IMessageRepository /*: IRepository<Message, Guid>*/
    {
        /// <summary>
        /// 创建消息
        /// </summary>
        /// <param name="message"></param>
        Message Register(Message message);
    }
}
