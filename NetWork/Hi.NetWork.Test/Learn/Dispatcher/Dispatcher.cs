using Hi.Infrastructure.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hi.NetWork.Test.Learn.Dispatcher {

    /// <summary>
    /// 分发器
    /// </summary>
    public class Dispatcher {

        protected ConcurrentDictionary<string, List<Action<IEvent>>> _handlers = new ConcurrentDictionary<string, List<Action<IEvent>>>();

        protected ConcurrentQueue<IEvent> _eventQueue = new ConcurrentQueue<IEvent>();

        private int _looping = 0;

        private object _sync = new object();

        public Dispatcher() {

        }

        /// <summary>
        /// 注册服务 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="handle"></param>
        public void Subscrible(string serverName, Action<IEvent> notification) {

            Ensure.IsNotOrEmpty(serverName, "serverName不能为空");
            Ensure.IsNotNull(notification, "notification不能为空");

            if (_handlers.ContainsKey(serverName)) {

                lock (_sync) {

                    _handlers[serverName].Add(notification);

                }

            } else {

                _handlers.TryAdd(serverName, new List<Action<IEvent>>() { notification });

            }

        }

        /// <summary>
        /// 解除服务
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="handle"></param>
        public void UnSubscrible(string serverName, Action<IEvent> notification) {

            if (_handlers.ContainsKey(serverName)) {

                var actions = _handlers[serverName];

                actions.Remove(actions.FirstOrDefault(ntf => ntf.Equals(notification)));

            }

        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="evt"></param>
        public void Publish(IEvent evt) {

            publish(evt);

            loop();

        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="evt"></param>
        protected void publish(IEvent evt) {

            Ensure.IsNotNull(evt, "event不能为空");

            _eventQueue.Enqueue(evt);

        }

        /// <summary>
        /// 事件循环
        /// </summary>
        protected void loop() {

            if (!inEventLoop()) return;
            if (_eventQueue.Count == 0) return;

            Task.Factory.StartNew(() => { 

                IEvent _evt; 

                while (_eventQueue.TryDequeue(out _evt)) { 

                    if (!_handlers.ContainsKey(_evt.ServerName)) break; 

                    var _hds = _handlers[_evt.ServerName]; 

                    if (_hds == null) break; 

                    _hds.ForEach(action => { action?.Invoke(_evt); }); 
                    
                } 

                exitEventLoop(); 
                
            });

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
