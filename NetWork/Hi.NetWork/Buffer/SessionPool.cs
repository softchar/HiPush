using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Hi.NetWork.Socketing;

namespace Hi.NetWork.Buffer
{
    
    /// <summary>
    /// 会员连接池,避免频繁创建会员导致的
    /// </summary>
    public class SessionPool {
        private Stack<TcpConnection> stack;
        public SessionPool(int capacity) {
            stack = new Stack<TcpConnection>(capacity);
        }

        public void Push(TcpConnection session) {
            lock (stack) {
                stack.Push(session);
            }
        }
        public TcpConnection Pop() {
            lock (stack) {
                return stack.Pop();
            }
        }

        public int Capacity {
            get {
                return stack.Count;
            }
        }
    }
}
