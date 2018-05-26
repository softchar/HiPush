using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Repository.Dapper.DataContextStorage
{
    public class DbTranContextFactory
    {
        //存储在缓存中的键,真实应该来自config
        private static readonly string key = "hi-db-tran";

        public static SqlTransaction GetSqlTransaction()
        {
            IStorageContainer<SqlTransaction> _container = DataContextStorageFactory<SqlTransaction>.CreateStorageContainer();

            var tran = _container.GetObject(key);
            if (tran == null)
            {
                tran = SqlConnectionContextFactory.GetSqlConnection().BeginTransaction();
                _container.Store(key, tran);
            }

            return tran;
        }
    }
}
