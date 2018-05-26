using Hi.Infrastructure.UnitOfWork;
using Hi.Model.Application;
using Hi.Server.Implement;
using Hi.Server.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebApp.App_Start;
using Hi.Repository.Dapper.UnitOfWork;
using Hi.Repository.Dapper.Repositories;
using Hi.Infrastructure.Configuration;
using Hi.Infrastructure.Ioc;
using Hi.Infrastructure.Messaging.Command;

namespace WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ControllActivator();
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        private void ControllActivator()
        {
            NinjectControllActivator controllerActivator = new NinjectControllActivator();
            DefaultControllerFactory controllerFactory = new DefaultControllerFactory(controllerActivator);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

    }
}
