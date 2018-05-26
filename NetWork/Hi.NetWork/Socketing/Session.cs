using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Socketing {

    using Channels;

    /// <summary>
    /// 会话管理器
    /// </summary>
    public class Session {

        List<IChannel> Connections;

        public Session() {
            Connections = new List<IChannel>();
        }

        public void Add(IChannel connection) {
            Connections.Add(connection);
        }
        public void Remove(IChannel connection) {
            Connections.Remove(connection);
        }

        public int Count {
            get {
                return Connections.Count;
            }
        }
    }
}
