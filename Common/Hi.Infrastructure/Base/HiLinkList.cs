using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Base
{
    public abstract class HiLinkList<T> : ILinkList<T>, IEnumerable<T>
        where T : ILinkNode<T>
    {

        T head;
        T tail;

        public T Head => head;

        public T Tail => tail;

        public HiLinkList()
        {
            //head = tail = default(T);
        }

        public void AddLast(T node)
        {
            if (IsEmpty())
            {
                this.head = node;
                this.tail = node;
            }
            else
            {
                var t = this.tail;
                t.SetNext(node);
                this.tail = node;
                this.tail.SetPrev(t);
            } 
        }

        public void DeleteFirst()
        {
            //如果head=tail=null则表示链表无节点直接返回
            if (IsEmpty())
            {
                return;
            }

            var _next = head.Next;
            
            //如果head节点的下一个为空，说明只有一个节点，那么删除节点就等于
            //把head和tail置空
            if (_next == null)
            {
                head = tail = default(T);
            }

            //否则，节点数大于1，只需要将head节点的下一个节点的prev置空，并且
            //把head节点的下一个节点设置为head节点就可以了。
            else
            {
                _next.SetPrev(default(T));
                head = _next;
            }
        }

        private bool IsEmpty() => head == null && tail == null;

        public IEnumerator<T> GetEnumerator()
        {
            for (T cur = head; cur != null;)
            {
                yield return cur;
                cur = cur.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public T this[int index]
        {
            get
            {
                if (IsEmpty() || index < 0)
                    throw new IndexOutOfRangeException();

                if (index == 0) return head;

                T h = head;

                for (int i = 0; i < index; i++)
                {
                    h = h.Next;
                    if (h == null)
                        throw new IndexOutOfRangeException();
                }

                return h;
            }
        }

        public int Count
        {
            get
            {
                //if (IsEmpty()) return 0;

                int index = 0;
                T h = head;
                while (h != null)
                {
                    index++;
                    h = h.Next;
                }

                return index;
            }
        }
    }
}
