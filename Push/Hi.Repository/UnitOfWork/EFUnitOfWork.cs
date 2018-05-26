using Hi.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Infrastructure.Domain;
using Hi.Repository.DataContextStorage;

namespace Hi.Repository.UnitOfWork
{
    public class EFUnitOfWork : IUnitOfWork
    {
        public void Commit()
        {
            DataContextFactory.GetDataContext().SaveChanges();
            DataContextFactory.GetDataContext().Dispose();
        }

        public void RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository unitOfWorkRepository)
        {
            unitOfWorkRepository.PersistUpdateOf(entity);
        }

        public void RegisterNew(IAggregateRoot entity, IUnitOfWorkRepository unitOfWorkRepository)
        {
            unitOfWorkRepository.PersistCreationOf(entity);
        }

        public void RegisterRemoved(IAggregateRoot entity, IUnitOfWorkRepository unitOfWorkRepository)
        {
            unitOfWorkRepository.PersistDeletionOf(entity);
        }
    }
}
