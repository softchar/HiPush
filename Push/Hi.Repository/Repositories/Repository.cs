using Hi.Infrastructure.Domain;
using Hi.Infrastructure.Querying;
using Hi.Infrastructure.UnitOfWork;
using Hi.Model.Pushing;
using Hi.Repository.DataContextStorage;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Repository.Repositories
{
    
    public class Repository<T, EntityKey> : IUnitOfWorkRepository, IRepository<T, EntityKey>
        where T : Entity,IAggregateRoot
    {
        protected IUnitOfWork _uow;

        public Repository(IUnitOfWork uow) {
            _uow = uow;
        }

        protected void SetUnitOfWork(IUnitOfWork uow) {
            _uow = uow;
        }

        public void Add(T entity) {
            _uow.RegisterNew(entity, this);
        }

        public void Remove(T entity) {
            _uow.RegisterRemoved(entity, this);
        }

        public void Save(T entity) {

        }

        /// <summary>
        /// 获得实体对象
        /// </summary>
        /// <returns></returns>
        public virtual DbSet<T> GetObjectSet() {
            return DataContextFactory.GetDataContext().Set<T>();
        }
        public virtual T FindBy(EntityKey Id) {
            return GetObjectSet().FirstOrDefault(entity => entity.Token.Equals(Id));
        }



        public virtual ObjectQuery<T> TranslateIntoObjectQueryFrom(Query query) {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindAll() {
            return GetObjectSet().ToList<T>();
        }

        public IEnumerable<T> FindAll(int index, int count) {
            return GetObjectSet().Skip(index).Take(count).ToList<T>();
        }

        public IEnumerable<T> FindBy(Query query) {
            ObjectQuery<T> efQuery = TranslateIntoObjectQueryFrom(query);
            return efQuery.ToList<T>();
        }

        public virtual IEnumerable<T> FindBy(Expression<Func<T, bool>> expression) {
            return GetObjectSet().Where(expression);
        }

        public IEnumerable<T> FindBy(Query query, int index, int count) {
            ObjectQuery<T> efQuery = TranslateIntoObjectQueryFrom(query);
            return efQuery.Skip(index).Take(count).ToList<T>();
        }



        public void PersistCreationOf(IAggregateRoot entity)
        {
            GetObjectSet().Add((T)entity);
        }

        public void PersistDeletionOf(IAggregateRoot entity)
        {
            var et = (T)entity;
            et.IsRemoved = true;
            DataContextFactory.GetDataContext().Entry<T>(et).State = EntityState.Modified;
        }

        public void PersistUpdateOf(IAggregateRoot entity)
        {
            var et = (T)entity;
            DataContextFactory.GetDataContext().Entry<T>(et).State = EntityState.Modified;
        }
    }
    
}
