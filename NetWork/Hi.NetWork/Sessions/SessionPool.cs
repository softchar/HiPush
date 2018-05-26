using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Hi.NetWork.Socketing;
using Hi.NetWork.Socketing.Channels;

namespace Hi.NetWork.Sessions
{
    
    /// <summary>
    /// 会员连接池,避免频繁创建会员导致的
    /// </summary>
    public class SessionPool {

        private Stack<IChannel> stack;

        public SessionPool(int capacity) {

            stack = new Stack<IChannel>(capacity);

        }

        public void Push(IChannel session) {

            lock (stack) {

                stack.Push(session);

            }

        }

        public IChannel Pop() {

            lock (stack) {

                return stack.Pop();

            }

        }

        public int Capacity {

            get { return stack.Count; }

        }
    }
}
