using Hi.Infrastructure.Domain;
using Hi.NetWork.Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Infrastructure.Querying;
using System.Linq.Expressions;
using Dapper;

namespace Hi.NetWork.Server.Repository {
    public class ReceiveLogRepository : IRepository<ReceiveLog, Guid> {
        public void Add(ReceiveLog entity) {

            string sql = "INSERT INTO [dbo].[ReceiveLog]([Token],[IP],[Port],[Data], [CreateTime], [LastUpdateTime]) VALUES(@Token, @IP, @Port, @Data, @CreateTime, @LastUpdateTime)";

            using (var conn = SqlConnectionContextFactory.GetSqlConnection()) {

                conn.ExecuteScalar<int>(sql, new { Token = entity.Token, IP = entity.IP, Port = entity.Port, Data = entity.Data, CreateTime = entity.CreateTime, LastUpdateTime = entity.LastUpdateTime });

            } 

        }

        public IEnumerable<ReceiveLog> FindAll() {
            throw new NotImplementedException();
        }

        public IEnumerable<ReceiveLog> FindBy(Query query) {
            throw new NotImplementedException();
        }

        public IEnumerable<ReceiveLog> FindBy(Expression<Func<ReceiveLog, bool>> expression) {
            throw new NotImplementedException();
        }

        public ReceiveLog FindBy(Guid token) {
            throw new NotImplementedException();
        }

        public IEnumerable<ReceiveLog> FindBy(Query query, int index, int count) {
            throw new NotImplementedException();
        }

        public void Remove(ReceiveLog entity) {
            throw new NotImplementedException();
        }

        public void Save(ReceiveLog entity) {
            throw new NotImplementedException();
        }
    }
}
