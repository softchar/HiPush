using Hi.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="unitOfWorkRepository"></param>
        void RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository unitOfWorkRepository);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="unitOfWorkRepository"></param>
        void RegisterNew(IAggregateRoot entity, IUnitOfWorkRepository unitOfWorkRepository);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="unitOfWorkRepository"></param>
        void RegisterRemoved(IAggregateRoot entity, IUnitOfWorkRepository unitOfWorkRepository);

        /// <summary>
        /// 提交
        /// </summary>
        void Commit();
    }
}
