using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.EventHandle {

    public class EventBus {

        private List<ISubscriber> _subscribers = new List<ISubscriber>();

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="subscriber"></param>
        public void Subscribe(ISubscriber subscriber) {
            if (!_subscribers.Contains(subscriber))
                _subscribers.Add(subscriber);
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="subscriber"></param>
        public void UnSubscribe(ISubscriber subscriber) {
            if (_subscribers.Contains(subscriber)) 
                _subscribers.Remove(subscriber);
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="evt"></param>
        public void Publish(IEvent evt) {
            Parallel.ForEach(_subscribers.Where(sub => sub.Code == evt.Code), sub => { sub.Handle(evt); });
        }
    } 
}
