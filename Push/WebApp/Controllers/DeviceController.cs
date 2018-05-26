using Hi.Infrastructure.Messaging.Command;
using Hi.Server.Interface;
using Hi.Server.Messaging.Commanding;
using Hi.Server.Messaging.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hi.Server.Messaging.Result;

namespace WebApp.Controllers {
    public class DeviceController : Controller {

        private CommandBus commandBus { get; set; }

        public DeviceController(CommandBus commandBus)
        {
            this.commandBus = commandBus;
        }

        /// <summary>
        /// 注册设备
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(RegisterDeviceCommand command)
        {
            var result = new RegisterDevicesResult();

            var commandResult = commandBus.Send(command);

            return new JsonResult() { Data = result.Create(commandResult.Values) };
        }
    }
}