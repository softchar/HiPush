using Hi.Model.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Hi.Infrastructure.UnitOfWork;
using Hi.Repository.Dapper.DataContextStorage;
using Dapper;
using Hi.Infrastructure.Domain;
using Hi.Infrastructure.Dto;

namespace Hi.Repository.Dapper.Repositories
{
    public class ApplicationRepository : Repository<Application, Guid>, IApplicationRepository
    {
        public ApplicationRepository(IUnitOfWork uow) : base(uow)
        {
        }
        

        public bool AppIdIsInvalid(Guid AppId)
        {
            string sql = "select count(*) from [application] where [appId] = @appId and [status] = @status";
            using (var conn = SqlConnectionContextFactory.GetSqlConnection()) {
                int i = conn.ExecuteScalar<int>(sql, new { appId = AppId, status = 0 });
                return i > 0;
            }
        }

        public bool AppNameIsInvalid(string appName)
        {
            string sql = "select count(*) from [application] where [appName] = @appName and [status] = @status";
            using (var conn = SqlConnectionContextFactory.GetSqlConnection())
            {
                int i = conn.ExecuteScalar<int>(sql, new { appName = appName, status = 0 });
                return i > 0;
            }
        }

        public bool IsHas(Application application)
        {
            return AppIdIsInvalid(application.AppId);
        }

        public Application Register(Application application)
        {
            Add(application);
            _uow.Commit();
            return application;
        }

        public override void PersistCreationOf(IAggregateRoot entity)
        {
            var application = (Application)entity;
            string sql = "insert into [application]([token],[appId],[appName],[createTime],[lastUpdateTime],[isRemoved],[status]) " +
                            "values(@token, @appId, @appName, @createTime, @lastUpdateTime, @isRemoved, @status)";
            var conn = SqlConnectionContextFactory.GetSqlConnection();
            conn.Execute(sql,
                new
                {
                    token = application.Token,
                    appId = application.AppId,
                    appName = application.AppName,
                    createTime = application.CreateTime,
                    lastUpdateTime = application.LastUpdateTime,
                    isRemoved = application.IsRemoved,
                    status = application.Status
                },
                transaction: DbTranContextFactory.GetSqlTransaction()
                );
        }

        public override void PersistDeletionOf(IAggregateRoot entity)
        {
            var application = (Application)entity;
            string sql = "update [application] set isremoved=1 where token=@token and appId = @appId";
            var conn = SqlConnectionContextFactory.GetSqlConnection();
            conn.Execute(sql, new { token = application.Token, appId = application.AppId }, DbTranContextFactory.GetSqlTransaction());
        }

        public override void PersistUpdateOf(IAggregateRoot entity)
        {
            var application = (Application)entity;
            string sql = "update [application] set [appId] = @appId,[appName] = @appName,[lastUpdateTime] = @lastUpdateTime,[isRemoved] = @isRemoved,[status] = @status where token=@token and appId=@appId";
            var conn = SqlConnectionContextFactory.GetSqlConnection();
            conn.Execute(sql, 
                new
                {
                    appId = application.AppId,
                    appName = application.AppName,
                    lastUpdateTime = application.LastUpdateTime,
                    isRemoved = application.IsRemoved,
                    status = application.Status,
                    token = application.Token
                }, 
                DbTranContextFactory.GetSqlTransaction()
                );
        }


        public bool IsInvalidByProperty(Expression<Func<Application, bool>> expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取application集合
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageResult<Application> GetApplications(string appName, int pageIndex, byte pageSize)
        {
            PageResult<Application> result = new PageResult<Application>() { PageIndex = pageIndex, PageSize = pageSize };

            string sql = @"select top(@pagesize) * from (
                            select top(@count1) *, ROW_NUMBER() over(order by[version] desc) as r from[application] where isRemoved = 0
                        ) t where t.r > @count2 ";
            string sqlc = @"select count(*) from[application] where isRemoved = 0";

            using (var conn = SqlConnectionContextFactory.GetSqlConnection()) {
                result.Data = conn.Query<Application>(sql, new { pagesize = pageSize, count1 = pageIndex * pageSize, count2 = (pageIndex-1) * pageSize }).ToList();
                result.Total = conn.ExecuteScalar<int>(sqlc);
                return result;
            }   
        }

        /// <summary>
        /// 删除引用程序
        /// </summary>
        /// <param name="application"></param>
        public void RemoveApplication(Application application)
        {
            Remove(application);
            _uow.Commit();
        }

        /// <summary>
        /// 修改应用程序
        /// </summary>
        /// <param name="application"></param>
        public void UpdateApplication(Application application)
        {
            Save(application);
            _uow.Commit();
        }
    }
}
