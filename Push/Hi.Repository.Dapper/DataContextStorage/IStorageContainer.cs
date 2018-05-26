using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Repository.Dapper.DataContextStorage
{
    public interface IStorageContainer<T>
    {
        /// <summary>
        /// 获得数据访问上下文
        /// </summary>
        /// <returns></returns>
        T GetObject(string key);

        /// <summary>
        /// 存储数据访问上下文
        /// </summary>
        /// <param name="context"></param>
        void Store(string key, T data);
    }
}
