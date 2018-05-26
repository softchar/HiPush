using Hi.Infrastructure.Domain;
using Hi.Model.Pushing.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Pushing
{
    public interface IPushMessageRepository /*: IRepository<MessageDevice, Guid>*/ {

        PushMessage Register(PushMessage pushMessage);

        /// <summary>
        /// 获得预推送的消息列表
        /// </summary>
        /// <param name="status"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<PushMessage> GetPrePushMessages(PushMessageStatus status, int size);
    }
}
