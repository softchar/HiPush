using Hi.Infrastructure.Configuration;
using Hi.Infrastructure.Ioc;
using Hi.Infrastructure.Messaging.Command;
using Hi.Infrastructure.Messaging.Event;
using Hi.Infrastructure.UnitOfWork;
using Hi.Model.Application;
using Hi.Repository.Dapper.Repositories;
using Hi.Repository.Dapper.UnitOfWork;
using Hi.Server.Implement;
using Hi.Server.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApp.App_Start {
    public class NinjectControllActivator : IControllerActivator {

        public NinjectControllActivator() {

            initMap();

        }

        public IController Create(RequestContext requestContext, Type controllerType) {

            return InterFaceFactory.Container.TryGet<IController>(controllerType);

        }

        private void initMap() {

            InterFaceFactory.Container.Register<IUnitOfWork, DapperUnitOfWork>();

            //为ApplicationRepository的构造函数的参数uow绑定指定的对象
            InterFaceFactory.Container.Register<IApplicationRepository, ApplicationRepository>();

            InterFaceFactory.Container.Register<IApplicationServer, ApplicationServer>();
            InterFaceFactory.Container.Register<IDeviceServer, DeviceServer>();

            InterFaceFactory.Container.Register<IApplicationSettings, WebConfigApplicationSettings>();

            InterFaceFactory.Container.RegisterInstance<ICommandHandleRegisterEntry, CommandHandleRegisterEntry>();
            InterFaceFactory.Container.RegisterInstance<IDomainEventHandleRegisterEntry, DomainEventHandleRegisterEntry>();
            InterFaceFactory.Container.RegisterInstance<CommandBus>();
            //InterFaceFactory.Container.RegisterInstanceWithConstructorArgument<CommandBus>("", InterFaceFactory.Container.Get<ICommandHandleRegisterEntry>()));
            




            ApplicationSettingsFactory.InitialzeApplicationSettingsFactory(InterFaceFactory.Container.Get<IApplicationSettings>());

        }
    }
}