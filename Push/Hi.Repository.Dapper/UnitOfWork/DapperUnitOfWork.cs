using Hi.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Infrastructure.Domain;
using Hi.Repository.Dapper.DataContextStorage;
using Dapper;
using System.Transactions;

namespace Hi.Repository.Dapper.UnitOfWork
{
    public class DapperUnitOfWork : IUnitOfWork
    {
        private Dictionary<IAggregateRoot, IUnitOfWorkRepository> Amended;
        private Dictionary<IAggregateRoot, IUnitOfWorkRepository> New;
        private Dictionary<IAggregateRoot, IUnitOfWorkRepository> Removed;

        public DapperUnitOfWork() {
            InitContainer();
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~DapperUnitOfWork() {
            CleanContainer();
        }

        /// <summary>
        /// 所有命令必须通过Commit才能提交
        /// </summary>
        public void Commit()
        {
            using (var conn = SqlConnectionContextFactory.GetSqlConnection())
            using (var dbtran = DbTranContextFactory.GetSqlTransaction())
            {
                foreach (IAggregateRoot entity in this.Amended.Keys)
                {
                    this.Amended[entity].PersistUpdateOf(entity);
                }

                foreach (IAggregateRoot entity in this.New.Keys)
                {
                    this.New[entity].PersistCreationOf(entity);
                }

                foreach (IAggregateRoot entity in this.Removed.Keys)
                {
                    this.Removed[entity].PersistDeletionOf(entity);
                }

                dbtran.Commit();
                CleanContainer();
            }
        }

        

        public void RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository unitOfWorkRepository)
        {
            if (!Amended.ContainsKey(entity)){
                Amended.Add(entity, unitOfWorkRepository);
            }
        }

        public void RegisterNew(IAggregateRoot entity, IUnitOfWorkRepository unitOfWorkRepository)
        {
            if (!New.ContainsKey(entity))
            {
                New.Add(entity, unitOfWorkRepository);
            }
        }

        public void RegisterRemoved(IAggregateRoot entity, IUnitOfWorkRepository unitOfWorkRepository)
        {
            if (!Removed.ContainsKey(entity))
            {
                Removed.Add(entity, unitOfWorkRepository);
            }
        }

        /// <summary>
        /// 初始化容器
        /// </summary>
        private void InitContainer()
        {
            Amended = new Dictionary<IAggregateRoot, IUnitOfWorkRepository>();
            New = new Dictionary<IAggregateRoot, IUnitOfWorkRepository>();
            Removed = new Dictionary<IAggregateRoot, IUnitOfWorkRepository>();
        }
        /// <summary>
        /// 清理容器
        /// </summary>
        private void CleanContainer()
        {
            Amended.Clear();
            New.Clear();
            Removed.Clear();
        }
    }
}
