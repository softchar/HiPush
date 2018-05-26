using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Server.Repository {
    public class SqlConnectionContextFactory {
        
        //存储在缓存中的键,真实应该来自config
        private static readonly string key = "hi-db-connstr";

        //链接字符串,来自于config
        private static readonly string conStr = Infrastructure.Configuration.ApplicationSettingsFactory.GetApplicationSettings().ConnectionString;

        public static SqlConnection GetSqlConnection() {

            var connection = new SqlConnection(conStr);

            connection.Open();

            return connection;            

        }
    }
}
