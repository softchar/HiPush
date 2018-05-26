using Hi.Infrastructure.Messaging.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Messaging.Commanding
{
    /// <summary>
    /// 更新Application命令
    /// </summary>
    public class UpdateApplicationCommand : ICommand
    {
        public Guid AppId { get; set; }
        public Guid AppToken { get; set; }
        public string AppName { get; set; }
    }
}
