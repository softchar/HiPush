using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Server.Messaging.Result;
using Hi.Server.Interface;

namespace Hi.Server.Implement
{
    using Messaging.Request;
    using Interface;
    using Hi.Model.Application;
    using Hi.Model.Application.BusinessRules;
    using Hi.Infrastructure.Domain;
    using Hi.Model;
    using Messaging.Result;
    using Messaging.Commanding;
    using Infrastructure.Messaging.Command;
    using Infrastructure.Dto;
    using Infrastructure.Messaging.Event;
    using Messaging.Eventing;

    public class ApplicationServer : AppServerBase<Application>, IApplicationServer
    {

        private IApplicationRepository _applicationRepository;
        private DomainEventBus _eventBus;

        public ApplicationServer(IApplicationRepository applicationRepository, DomainEventBus eventBus) {
            _applicationRepository = applicationRepository;
            _eventBus = eventBus;
        }

        /// <summary>
        /// 新建Application
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ReturnValue Create(CreateApplicationCommand command)
        {
            var result = new ReturnValue();

            //初始化一个application对象
            var application = initApplication(command.AppName);

            try
            {
                //添加到资源库之前业务规则验证
                Validate(application);

                //检查App是否已存在
                IsHasApplicationByAppName(application.AppName);

                _applicationRepository.Register(application);

            }
            catch (BusinessRuleException excep)
            {
                return excep.ReturnValue;
            }

            return result;

        }

        /// <summary>
        /// 获取Application集合
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PageResult<Application> GetAllApplication(GetAllApplicationRequest request)
        {
            var apps = _applicationRepository.GetApplications(request.AppName, request.PageIndex, request.PageSize);
            return apps;
        }

        /// <summary>
        /// 删除应用程序
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public ReturnValue Remove(RemoveApplicationCommand command)
        {
            var result = new ReturnValue();

            try
            {
                var application = initApplication(command.AppId, command.AppToken);

                //添加到资源库之前业务规则验证
                //Validate(application);

                _applicationRepository.RemoveApplication(application);

                //发送应用程序删除的事件
                _eventBus.Send(new ApplicationRemovedEvent() { AppId = application.AppId });

            }
            catch (BusinessRuleException excep)
            {
                // TODO...
                return excep.ReturnValue;
            }

            return result;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ReturnValue Update(UpdateApplicationCommand command)
        {
            var result = new ReturnValue();

            try
            {
                var application = new Application() { AppId = command.AppId, AppName = command.AppName, Token = command.AppToken };

                Validate(application);

                _applicationRepository.UpdateApplication(application);
            }
            catch (BusinessRuleException excep)
            {
                return excep.ReturnValue;
            }

            return result;
        }

        /// <summary>
        /// 初始化application对象
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        /// <remarks>这个方法设置成virtual是方便做单元测试</remarks>
        public virtual Application initApplication(string appName)
        {
            var app = new Application() { AppName = appName };
            return app;
        }

        public virtual Application initApplication(Guid AppId) {
            var app = new Application() { AppId = AppId };
            return app;
        }

        public virtual Application initApplication(Guid AppId, Guid Token) {
            var app = new Application() { AppId = AppId, Token = Token };
            return app;
        }

        /// <summary>
        /// 判断指定AppName是否已存在
        /// </summary>
        /// <param name="AppName"></param>
        /// <returns></returns>
        private bool IsHasApplication(Application application)
        {
            bool isHas = _applicationRepository.IsHas(application);
            if (isHas) {
                //var retValue = new CreateApplicationRetValue();
                var retValue = new ReturnValue();
                retValue.Set((byte)CreateApplicationCode.AppIdHad, ApplicationBusinessRule.AppIdIsHad);
                throw BusinessRuleException.Create(retValue);
            }
            return isHas;
        }

        private bool IsHasApplicationByAppName(string appName)
        {
            bool isHas = _applicationRepository.AppNameIsInvalid(appName);
            if (isHas)
            {
                var retValue = new ReturnValue();
                retValue.Set((byte)CreateApplicationCode.AppNameIsHad, ApplicationBusinessRule.AppNameIsHad);
                throw BusinessRuleException.Create(retValue);
            }
            return isHas;
        }

    }
}
