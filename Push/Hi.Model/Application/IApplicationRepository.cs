
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Hi.Infrastructure.Domain;
using Hi.Infrastructure.UnitOfWork;
using Hi.Infrastructure.Dto;

namespace Hi.Model.Application
{
    public interface IApplicationRepository 
    {
        bool IsHas(Application application);

        /// <summary>
        /// 判断指定AppName的Application是否存在
        /// </summary>
        /// <param name="appName"></param>
        bool AppNameIsInvalid(string appName);

        /// <summary>
        /// 判断AppId是否有效
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        bool AppIdIsInvalid(Guid AppId);

        bool IsInvalidByProperty(Expression<Func<Application, bool>> expression);

        /// <summary>
        /// 注册应用程序
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        Application Register(Application application);

        /// <summary>
        /// 删除应用程序
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        void RemoveApplication(Application application);

        /// <summary>
        /// 更新引用程序
        /// </summary>
        /// <param name="application"></param>
        void UpdateApplication(Application application);

        /// <summary>
        /// 获得应用程序
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PageResult<Application> GetApplications(string appName, int pageIndex, byte pageSize);
    }
}
