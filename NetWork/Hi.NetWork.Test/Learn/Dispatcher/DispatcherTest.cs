using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Hi.Infrastructure.Base;

namespace Hi.NetWork.Test.Learn.Dispatcher {

    [TestClass]
    public class DispatcherTest {

        /************************************************************************/
        /* TODO  
         * 
         *  功能
         *      1,注册服务
         *      2,解除服务
         *      3,Eventloop
         *      4,Event Handle                                                
         *      
         *  测试列表
         *      1,输入名字服务serverName=100, 分发器dispatcher存在一个100的项
         *      2,输入一个已存在名字服务的serverName=100, 分发器dispatcher的项数量>=2
         *      3,注册名字服务serverName=100, 解除服务100, 分发器的dispatcher内没有刚才注册的服务
         *      4,解除一个已有多个处理函数的名字服务，不会影响其他的订阅服务。
         *          注册名字服务serverName=100, notification=handle
         *          注册名字服务serverName=100, notification=handle2
         *          取消名字服务serverName=100, notification=handle, 名字服务serverName=100剩余notification=handle2
         *         
         *      5,serverName=""报异常, notification为null报异常
         *      
         *      6,发布事件,event=null报异常
         *      7,发布事件,输入event对象,队列长度为1 
         *      
         *      8,event loop
         *      
         *                                                                      */
        /************************************************************************/

        /// <summary>
        /// 1,输入名字服务serverName=100, 分发器dispatcher存在一个100的项
        /// 3,注册名字服务serverName=100, 解除服务100, 分发器的dispatcher内没有刚才注册的服务
        /// </summary>
        [TestMethod]
        public void dispatcher_register_and_unregister_test() {

            var serverName = "100";

            var dispatcher = new DispatcherFake();

            /************************************************************************/
            /* 订阅
             * 1,输入名字服务serverName=100, 分发器dispatcher存在一个100的项
             *                                                                      */
            /************************************************************************/
            dispatcher.Subscrible(serverName, handle);

            bool isHasServer;

            isHasServer = dispatcher.TryGet(serverName, handle);

            Assert.IsTrue(isHasServer);

            /************************************************************************/
            /* 取消订阅
             * 3,注册名字服务serverName=100, 解除服务100, 分发器的dispatcher内没有刚才注册的服务
             *                                                                      */
            /************************************************************************/
            dispatcher.UnSubscrible(serverName, handle);

            isHasServer = dispatcher.TryGet(serverName, handle);

            Assert.IsTrue(!isHasServer);

        }

        /// <summary>
        /// 
        /// 2,输入一个已存在名字服务的serverName=100, 分发器dispatcher的项数量>=2
        /// 
        /// 4,解除一个已有多个处理函数的名字服务，不会影响其他的订阅服务。
        ///     注册名字服务serverName=100, notification=handle
        ///     注册名字服务serverName = 100, notification = handle2
        ///     取消名字服务serverName = 100, notification = handle, 名字服务serverName = 100剩余notification=handle2
        /// </summary>
        [TestMethod]
        public void dispatcher_register_and_unregister_mul_test() {

            var _serverName = "100";

            var _dispatcher = new DispatcherFake();

            /************************************************************************/
            /* 订阅
             * 2,输入一个已存在名字服务的serverName=100, 分发器dispatcher的项数量>=2
             *                                                                      */
            /************************************************************************/
            _dispatcher.Subscrible(_serverName, handle);
            _dispatcher.Subscrible(_serverName, handle2);

            bool isHasServer, isHasServer2;

            isHasServer = _dispatcher.TryGet(_serverName, handle);
            isHasServer2 = _dispatcher.TryGet(_serverName, handle2);

            Assert.IsTrue(isHasServer);
            Assert.IsTrue(isHasServer2);

            Assert.AreEqual(_dispatcher.HandleCount(_serverName), 2);


            /************************************************************************/
            /* 取消订阅
             * 4,解除一个已有多个处理函数的名字服务，不会影响其他的订阅服务。
                    注册名字服务serverName=100, notification=handle
                    注册名字服务serverName = 100, notification = handle2
                    取消名字服务serverName = 100, notification = handle, 名字服务serverName = 100剩余notification=handle2
             *                                                                      */
            /************************************************************************/
            _dispatcher.UnSubscrible(_serverName, handle);

            isHasServer = _dispatcher.TryGet(_serverName, handle);
            isHasServer2 = _dispatcher.TryGet(_serverName, handle2);

            Assert.IsTrue(!isHasServer);
            Assert.IsTrue(isHasServer2);

        }

        /// <summary>
        /// 5,serverName=""，报异常
        /// </summary>
        [TestMethod]
        public void dispatcher_register_serverName_exception_test() {

            var _dispatcher = new Dispatcher();

            try {

                _dispatcher.Subscrible(string.Empty, handle);

                Assert.Fail();

            } catch (Exception excep) {

                Assert.IsTrue(excep.Message.Contains("serverName"));

            }

            try {

                _dispatcher.Subscrible("100", null);

                Assert.Fail();

            } catch (Exception excep) {

                Assert.IsTrue(excep.Message.Contains("notification"));

            }

        }

        /// <summary>
        /// 6,发布事件,event=null报异常
        /// </summary>
        [TestMethod]
        public void dispatcher_publish_serverName_exception_test() {

            var _dispatcher = new DispatcherFake();

            try {

                _dispatcher.publish(null);

                Assert.Fail();

            } catch (Exception excep) {

                Assert.IsTrue(excep.Message.Contains("event"));

            }

        }

        /// <summary>
        /// 7,发布事件,输入event对象,队列长度为1 
        /// </summary>
        [TestMethod]
        public void dispatcher_publish_test() {

            var _dispatcher = new DispatcherFake();

            _dispatcher.publish(new Event("100"));

            var len = _dispatcher.QueueLength();

            Assert.AreEqual(len, 1);

        }

        /*
        /// <summary>
        /// 
        /// </summary>
        public void dispatcher_event_loop_test() {

            var _dispatcher = new DispatcherFake();



        }
        */

        private Action<IEvent> handle = evt => {

            Console.WriteLine(@"Thread:{0}; ServerName:{1}", Thread.CurrentThread.ManagedThreadId, evt.ServerName);

        };

        private Action<IEvent> handle2 = evt => {

            Console.WriteLine(@"Thread:{0}; ServerName:{1}", Thread.CurrentThread.ManagedThreadId, evt.ServerName);

        };

        /// <summary>
        /// Dispatcher测试替身
        /// </summary>
        class DispatcherFake : Dispatcher {

            public void publish(IEvent evt){

                base.publish(evt);

            }
            public void loop() {

                base.loop();

            }

            public int HandleCount(string serverName) {

                if (!base._handlers.ContainsKey(serverName)) return 0;

                return base._handlers[serverName].Count();

            }

            public int QueueLength() {

                return base._eventQueue.Count;

            }

            public bool TryGet(string serverName, Action<IEvent> handle) {

                return _handlers.ContainsKey(serverName) && _handlers[serverName].Contains(handle);

            }

        }

        /// <summary>
        /// Event
        /// </summary>
        class Event : IEvent {

            private string _serverName = string.Empty;

            public string ServerName {
                get { return _serverName; }
                set { _serverName = value; }
            }

            public Event(string serverName) {

                Ensure.IsNotOrEmpty(serverName);

                _serverName = serverName;

            }

        }

    }
}
