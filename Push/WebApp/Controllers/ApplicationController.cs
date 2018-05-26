using Hi.Server.Interface;
using Hi.Server.Messaging.Commanding;
using Hi.Server.Messaging.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hi.Infrastructure.Messaging.Command;
using Hi.Infrastructure.Ioc;
using Hi.Server.Messaging.Result;

namespace WebApp.Controllers {
    public class ApplicationController : Controller {

        public IApplicationServer _applicationServer { get; private set; }

        public CommandBus _commandBus { get; private set; }

        public ApplicationController(IApplicationServer applicationServer, CommandBus bus) {

            _applicationServer = applicationServer;
            _commandBus = bus;

        }

        // GET: Application
        [HttpGet]
        public ActionResult Index() {

            return View();

        }

        [HttpPost]
        public JsonResult GetApps(GetAllApplicationRequest request) {

            var result = _applicationServer.GetAllApplication(request);

            return new JsonResult() { Data = result };

        }

        [HttpGet]
        public ActionResult Register() {

            return View();

        }

        /// <summary>
        /// 注册APP
        /// </summary>
        /// <param name="createApplicationCommand"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(CreateApplicationCommand createApplicationCommand)
        {
            var result = new CreateApplicationResult();

            var commandResult = _commandBus.Send(createApplicationCommand);

            return new JsonResult() { Data = result.Create(commandResult.Values) };

        }

        /// <summary>
        /// 删除APP
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Remove(RemoveApplicationCommand command) {

            var result = new RemoveApplicationResult();

            var commandResult = _commandBus.Send(command);

            return new JsonResult() { Data = result };

        }

        /// <summary>
        /// 更新APP
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Update(UpdateApplicationCommand command) {

            var result = new UpdateApplicationResult();

            var commandResult = _commandBus.Send(command);

            return new JsonResult() { Data = result.Create(commandResult.Values) };

        }

    }
}