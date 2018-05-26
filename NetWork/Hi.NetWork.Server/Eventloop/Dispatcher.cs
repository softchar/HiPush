using Hi.Infrastructure.Base;
using Hi.NetWork.Protocols;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hi.NetWork.Server.Eventloop {
    /// <summary>
    /// 分发器
    /// </summary>
    public class Dispatcher {

        protected ConcurrentDictionary<EventType, List<Action<HiEvent>>> _handlers = new ConcurrentDictionary<EventType, List<Action<HiEvent>>>();

        protected ConcurrentQueue<HiEvent> _eventQueue = new ConcurrentQueue<HiEvent>();

        private int _looping = 0;

        private object _sync = new object();

        public Dispatcher() {

        }

        /// <summary>
        /// 注册服务 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="handle"></param>
        public void Subscrible(EventType serverName, Action<HiEvent> notification) {

            Ensure.IsNotNull(notification, "notification不能为空");

            if (_handlers.ContainsKey(serverName)) {

                lock (_sync) {

                    _handlers[serverName].Add(notification);

                }

            } else {

                _handlers.TryAdd(serverName, new List<Action<HiEvent>>() { notification });

            }

        }

        /// <summary>
        /// 解除服务
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="handle"></param>
        public void UnSubscrible(EventType serverName, Action<HiEvent> notification) {

            if (_handlers.ContainsKey(serverName)) {

                var actions = _handlers[serverName];

                actions.Remove(actions.FirstOrDefault(ntf => ntf.Equals(notification)));

            }

        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="evt"></param>
        public void Publish(HiEvent evt) {

            publish(evt);

            loop();

        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="evt"></param>
        protected void publish(HiEvent evt) {

            Ensure.IsNotNull(evt, "event不能为空");

            _eventQueue.Enqueue(evt);

        }

        /// <summary>
        /// 事件循环
        /// </summary>
        protected void loop() {

            if (!inEventLoop()) return;
            if (_eventQueue.Count == 0) return;

            HiEvent _evt;

            while (_eventQueue.TryDequeue(out _evt)) {

                if (!_handlers.ContainsKey(_evt.Type)) break;

                var _hds = _handlers[_evt.Type];

                if (_hds == null) break;

                foreach (var handle in _hds) {
                    
                    handle?.Invoke(_evt);

                }

            } 

            exitEventLoop();

        }

        /// <summary>
        /// 是否开始事件循环
        /// </summary>
        /// <returns></returns>
        protected bool inEventLoop() {

            return Interlocked.CompareExchange(ref _looping, 1, 0) == 0;

        }

        /// <summary>
        /// 退出事件循环
        /// </summary>
        protected void exitEventLoop() {

            Interlocked.Exchange(ref _looping, 0);

        }

    }
}
