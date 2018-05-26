using Hi.Infrastructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Eventloops {
    public class AbstractEventInvoker {

        SingleThreadEventloop eventloop;

        /// <summary>
        /// 关联到Eventloop
        /// </summary>
        /// <param name="eventloop"></param>
        public void AssociateToEventloop(SingleThreadEventloop eventloop) {

            Ensure.IsNotNull(eventloop);

            this.eventloop = eventloop;
        }

        protected  void execute(Action action) {

            Ensure.IsNotNull(action);
            Ensure.IsNotNull(eventloop);

            if (eventloop.InEventloop) {
                action();
            } else {
                eventloop.Execute(new ActionTask(action));
            }

        }

        protected void execute(Task task) {
            Ensure.IsNotNull(task);
            Ensure.IsNotNull(eventloop);

            if (eventloop.InEventloop) {

            } else {
                //eventloop.Execute()
            }
        }
    }
}
