using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Infrastructure.Messaging.Command;

namespace Hi.Server.Messaging.Commanding
{
    /// <summary>
    /// 删除Application命令
    /// </summary>
    public class RemoveApplicationCommand : ICommand
    {
        public Guid AppId { get; set; }
        public Guid AppToken { get; set; }
    }
}
