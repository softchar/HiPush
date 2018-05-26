using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Repository.Dapper.DataContextStorage
{
    public class SqlConnectionContextFactory
    {
        //存储在缓存中的键,真实应该来自config
        private static readonly string key = "hi-db-connstr";
        //链接字符串,应来自于config
        //private static readonly string conStr = "data source=TOM-PC;initial catalog=HiPush;user id=sa;password=sasa;";

        private static readonly string conStr = Infrastructure.Configuration.ApplicationSettingsFactory.GetApplicationSettings().ConnectionString;

        public static SqlConnection GetSqlConnection()
        {
            

            IStorageContainer<SqlConnection> _container = DataContextStorageFactory<SqlConnection>.CreateStorageContainer();

            var _connection = _container.GetObject(key);
            if (_connection == null || _connection.State == System.Data.ConnectionState.Closed)
            {
                _connection = new SqlConnection(conStr);
                _connection.Open();
                _container.Store(key, _connection);
            }
            return _connection;
        }
    }
}
