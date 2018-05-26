using Hi.Infrastructure.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Domain
{
    public interface IReadOnlyRepository<T,Token> where T : IAggregateRoot
    {
        /// <summary>
        /// 通过标识获取
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        T FindBy(Token token);

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> FindAll();

        /// <summary>
        /// 更具查询条件获取
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IEnumerable<T> FindBy(Query query);

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="query"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<T> FindBy(Query query, int index, int count);

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IEnumerable<T> FindBy(Expression<Func<T, bool>> expression);
    }
}
