using Hi.Infrastructure.Domain;
using Hi.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Infrastructure.Querying;
using System.Linq.Expressions;
using Hi.Infrastructure.Reflection;

namespace Hi.Repository.Dapper.Repositories
{

    public class Repository<T, EntityKey> : IUnitOfWorkRepository, IRepository<T, EntityKey>
        where T : Entity, IAggregateRoot
    {
        protected IUnitOfWork _uow;

        public Repository(IUnitOfWork uow) 
        {
            _uow = uow;
        }


        public void Add(T entity)
        {
            _uow.RegisterNew(entity, this);
        }

        public void Remove(T entity)
        {
            _uow.RegisterRemoved(entity, this);
        }

        public void Save(T entity)
        {
            _uow.RegisterAmended(entity, this);
        }


        public virtual void PersistCreationOf(IAggregateRoot entity)
        {
            string[] props = HiTyper.GetSettableSimplePropNames(typeof(T));

            throw new NotImplementedException();
        }

        public virtual void PersistDeletionOf(IAggregateRoot entity)
        {
            throw new NotImplementedException();
        }

        public virtual void PersistUpdateOf(IAggregateRoot entity)
        {
            throw new NotImplementedException();
        }



        public IEnumerable<T> FindAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindBy(Query query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public T FindBy(EntityKey token)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindBy(Query query, int index, int count)
        {
            throw new NotImplementedException();
        }

    }
}
