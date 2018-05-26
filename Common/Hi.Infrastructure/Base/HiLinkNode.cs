using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Base
{
    public class HiLinkNode<T> : ILinkNode<T>
        where T : ILinkNode<T>
    {
        T prev;
        T next;
        public T Prev => prev;

        public T Next => next;

        public void SetNext(T item)
        {
            this.next = item;
        }

        public void SetPrev(T item)
        {
            this.prev = item;
        }
    }
}
