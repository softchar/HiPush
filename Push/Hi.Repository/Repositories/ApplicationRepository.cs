using Hi.Model.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Infrastructure.UnitOfWork;
using Hi.Infrastructure.Querying;
using System.Data.Entity.Core.Objects;
using Hi.Repository.DataContextStorage;
using Hi.Model;
using Hi.Infrastructure.Domain;
using System.Linq.Expressions;
using Hi.Infrastructure.Dto;

namespace Hi.Repository.Repositories
{

    public class ApplicationRepository : Repository<Application, Guid>, IApplicationRepository
    {
        public ApplicationRepository(IUnitOfWork uow) : base(uow)
        {

        }

        public bool IsInvalidByProperty(Expression<Func<Application, bool>> expression)
        {
            var count = DataContextFactory.GetDataContext().Application.Where(expression).Count();
            return count > 0;
        }

        public bool AppNameIsInvalid(string appName)
        {
            var count = DataContextFactory.GetDataContext().Application.Where(app => app.AppName.Equals(appName)).Count();
            return count > 0;
        }

        /// <summary>
        /// 检查Application是否已存在
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public bool IsHas(Application application)
        {
            var _app = DataContextFactory.GetDataContext().Application.Where(app => app.AppId == application.AppId).FirstOrDefault();
            return _app != null;
        }

        /// <summary>
        /// 注册Application
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Application Register(Application application)
        {
            Add(application);
            _uow.Commit();
            return application;
        }

        /// <summary>
        /// 判断AppId是否无效
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public bool AppIdIsInvalid(Guid AppId)
        {
            var count = DataContextFactory.GetDataContext().Application.Where(app => app.AppId.Equals(AppId)).Count();
            return count > 0;
        }

        /// <summary>
        /// 获取AppName集合
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        public PageResult<Application> GetApplications(string appName, int pageIndex, byte pageSize)
        {
            throw new NotImplementedException();
        }

        public ReturnValue Delete(Application application)
        {
            throw new NotImplementedException();
        }

        public void RemoveApplication(Application application)
        {
            throw new NotImplementedException();
        }

        public void UpdateApplication(Application application)
        {
            throw new NotImplementedException();
        }
    }

}
