using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Repository.DataContextStorage
{
    public interface IDataContextStorageContainer
    {
        /// <summary>
        /// 获得数据访问上下文
        /// </summary>
        /// <returns></returns>
        HiDataContext GetDataContext();

        /// <summary>
        /// 存储数据访问上下文
        /// </summary>
        /// <param name="context"></param>
        void Store(HiDataContext context);
    }
}
