using Hi.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.UnitOfWork
{
    public interface IUnitOfWorkRepository
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        void PersistCreationOf(IAggregateRoot entity);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        void PersistUpdateOf(IAggregateRoot entity);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        void PersistDeletionOf(IAggregateRoot entity);
    }
}
