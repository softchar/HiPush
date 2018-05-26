using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Infrastructure.Messaging.Command;

namespace Hi.Server.Messaging.Commanding
{
    /// <summary>
    /// 创建Application命令
    /// </summary>
    public class CreateApplicationCommand : ICommand
    {
        public string AppName { get; set; }
    }
}
