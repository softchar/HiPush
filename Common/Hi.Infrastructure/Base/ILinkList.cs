using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Base
{
    /// <summary>
    /// 链表
    /// </summary>
    public interface ILinkList<T>
        where T : ILinkNode<T>
    {

        T Head { get; }

        T Tail { get; }

        int Count { get; }

        void AddLast(T node);
        
    }
}
