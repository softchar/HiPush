using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.EventHandle {
    public interface ISubscriber  {

        byte Code { get; set; }

        /// <summary>
        /// 服务处理事件
        /// </summary>
        /// <param name="e"></param>
        void Handle(IEvent e);
    }
}
