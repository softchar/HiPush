using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Infrastructure.Domain;

namespace Hi.Server.Interface
{
    using Infrastructure.Dto;
    using Messaging.Commanding;
    using Messaging.Request;
    using Messaging.Result;
    using Model.Application;

    public interface IApplicationServer
    {
        /// <summary>
        /// 新建Application
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        ReturnValue Create(CreateApplicationCommand command);

        /// <summary>
        /// 获取Application集合
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PageResult<Application> GetAllApplication(GetAllApplicationRequest request);

        /// <summary>
        /// 删除应用程序
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        ReturnValue Remove(RemoveApplicationCommand command);

        /// <summary>
        /// 更新应用程序
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ReturnValue Update(UpdateApplicationCommand command);
    }
}
