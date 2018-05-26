using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Domain
{
    public interface IRepository<T, Token> : IReadOnlyRepository<T, Token>
        where T : IAggregateRoot
    {
        void Save(T entity);

        void Add(T entity);

        void Remove(T entity);
    }
}
