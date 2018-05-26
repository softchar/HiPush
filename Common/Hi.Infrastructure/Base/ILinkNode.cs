using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Base
{
    public interface ILinkNode<T>
    {
        T Prev { get; }

        T Next { get; }

        void SetPrev(T item);

        void SetNext(T item);

    }

}
