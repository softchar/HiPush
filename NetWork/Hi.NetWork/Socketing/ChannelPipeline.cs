using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.Collections;
using System.Reflection;
using Hi.Infrastructure.Base;
using System.Collections.Concurrent;
using Hi.NetWork.Socketing.Channels;

namespace Hi.NetWork.Socketing {
    public class ChannelPipeline : IEnumerable<IChannelHandler>, IChannelPipeline {

        //public IChannelHandler Tail { get; protected set; }

        public IChannelHandler Head { get; protected set; }

        private IChannelHandler Last { get; set; }

        /// <summary>
        /// 实现的函数
        /// </summary>
        private ConcurrentDictionary<string, List<string>> _direcImplMethods = new ConcurrentDictionary<string, List<string>>();

        public ChannelPipeline() {

            //Head = Tail = new TailChannelHandler();
            AddLast(new TailChannelHandler());

        }
        //public ChannelPipeline(IChannel channel) {

        //    Ensure.IsNotNull(channel);

        //    //this.channel = channel;

        //}

        //public IChannel channel { get; protected set; }

        public void AddLast(IChannelHandler next) {

            Contract.Ensures(next != null);

            if (Head == null) {

                Head = next;
                Last = next;

            } else {

                next.SetNext(Head);
                Head = next;

            }

            keepingImplMethods(next);

        }

        public void OnConnected(ChannelPipelineContext ctx) {

            handlerInvoke(handle => {

                if (isImplMethod(handle, "OnConnected"))
                    return handle.OnConnected(ctx.Channel);

                return true;

            });
        } 

        public void OnReceived(ChannelPipelineContext ctx, ChannelMessage message) {

            Ensure.IsNotNull(message);

            handlerInvoke(handle => {

                if (isImplMethod(handle, "OnReceived"))
                    return handle.OnReceived(ctx.Channel, message);

                return true;

            });

        }

        public void OnSend(ChannelPipelineContext ctx) {

            handlerInvoke(handle => {
                if (isImplMethod(handle, "OnSend"))
                    return handle.OnSend(ctx.Channel);
                return true;
            });

        }

        public void OnBreaked(ChannelPipelineContext ctx) {

            handlerInvoke(null);

        }

        public void OnReconnected(ChannelPipelineContext ctx) {

            handlerInvoke(null);

        }

        public IEnumerator<IChannelHandler> GetEnumerator() {

            var current = Head;

            do {

                yield return current;

            } while ((current = current.Next()) != null);

        }

        private void handlerInvoke(Func<IChannelHandler, bool> handlerInvoker) {

            if (handlerInvoker == null)

                return;

            foreach (var item in this) {

                if (!(bool)(handlerInvoker?.Invoke(item)))
                    break;

            }

        }

        private IChannelHandler GetNext(IChannelHandler handler) {

            Ensure.IsNotNull(handler);

            return handler.Next();

        }

        //public void SetChannel(IChannel channel) {

        //    Ensure.IsNotNull(channel);

        //    this.channel = channel;

        //}

        //public IChannel Channel() {

        //    return this.channel;

        //}

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 保存实现IChannelHandler的函数
        /// </summary>
        /// <param name="next"></param>
        private void keepingImplMethods(IChannelHandler next) {

            Ensure.IsNotNull(next);

            var _methods = next.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

            if (_methods.Count() <= 0) return;

            var _typeName = next.GetType().FullName;

            //需要过滤掉没有实现的函数，只留下实现的函数
            if (_direcImplMethods.ContainsKey(_typeName)) {

                foreach (var _method in _methods) 
                    _direcImplMethods[_typeName].Add(_method.Name);
                

            } else {

                List<string> _methodList = new List<string>();

                foreach (var item in _methods) 
                    _methodList.Add(item.Name);

                _direcImplMethods[_typeName] = _methodList;

            }
        }

        /// <summary>
        /// 判断当前的IChannelHandler是否已实现了指定名称的函数
        /// </summary>
        /// <param name="channelHandler"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private bool isImplMethod(IChannelHandler channelHandler, string methodName) {

            Ensure.IsNotNull(channelHandler);
            Ensure.IsNotOrEmpty(methodName);

            var _typeName = channelHandler.GetType().FullName;

            if (_direcImplMethods.ContainsKey(_typeName)) {

                var _methods = _direcImplMethods[_typeName];

                if (_methods == null || _methods.Count() <= 0)
                    return false;

                return _methods.Count(name => name == methodName) > 0;

            }

            return false;
        }

        /// <summary>
        /// 尾部处理函数
        /// </summary>
        class TailChannelHandler : ChannelHandler {
           

            public override bool OnReceived(IChannel channel, ChannelMessage message) {

                //释放消息
                //message.Return();

                return true;

            }

        }

    }
}
