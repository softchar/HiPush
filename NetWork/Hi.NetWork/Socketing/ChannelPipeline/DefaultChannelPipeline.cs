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
using Hi.NetWork.Buffer;
using System.Net;

namespace Hi.NetWork.Socketing.ChannelPipeline
{
    public class DefaultChannelPipeline : IEnumerable<IChannelHandler>, IChannelPipeline
    {

        /// <summary>
        /// Channel
        /// </summary>
        private IChannel channel;
        private IByteBufAllocator alloc;
        private ChannelPipelineContext context;

        public IChannelHandlerContext Head { get; protected set; }

        private IChannelHandlerContext Tail { get; set; }

        public IByteBufAllocator Alloc
        {
            get { return alloc; }
        }

        /// <summary>
        /// 实现的函数
        /// </summary>
        private ConcurrentDictionary<string, List<string>> _direcImplMethods = new ConcurrentDictionary<string, List<string>>();

        public DefaultChannelPipeline()
        {
            context = new ChannelPipelineContext();
            Head = new HeadChannelContext();
            Tail = new TailChannelContext();

            Head.Next = Tail;
            Tail.Prev = Head;
        }

        public DefaultChannelPipeline(IChannel channel)
            : this()
        {
            Ensure.IsNotNull(channel);

            this.channel = channel;

            this.context.SetChannel(channel);

        }

        public void AddLast(string name, IChannelHandler handler)
        {

            Ensure.IsNotOrEmpty(name);
            Ensure.IsNotNull(handler);

            var ctx = NewContext(name, channel, handler);

            Tail.Prev.Next = ctx;
            ctx.Prev = Tail.Prev;
            ctx.Next = Tail;
            Tail.Prev = ctx;

            //Tail.Next = ctx;
            //ctx.Prev = Tail;
            //Tail = ctx;

            //keepingImplMethods(ctx);

        }

        protected virtual IChannelHandlerContext NewContext(string name, IChannel channel, IChannelHandler handler)
        {
            var ctx = new ChannelHandlerContext(name, this, channel, handler);
            return ctx;
        }

        public void SetChannel(IChannel channel)
        {
            this.channel = channel;
            this.context.SetChannel(channel);
        }

        public void SetAlloc(IByteBufAllocator alloc)
        {
            this.alloc = alloc;
        }

        public IEnumerator<IChannelHandler> GetEnumerator()
        {

            //var current = Head;

            //do
            //{

            //    yield return current;

            //} while ((current = current.Next()) != null);

            return null;

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 保存实现IChannelHandler的函数
        /// </summary>
        /// <param name="next"></param>
        private void keepingImplMethods(IChannelHandler next)
        {

            Ensure.IsNotNull(next);

            var _methods = next.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

            if (_methods.Count() <= 0) return;

            var _typeName = next.GetType().FullName;

            //需要过滤掉没有实现的函数，只留下实现的函数
            if (_direcImplMethods.ContainsKey(_typeName))
            {

                foreach (var _method in _methods)
                    _direcImplMethods[_typeName].Add(_method.Name);
            }
            else
            {
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
        private bool isImplMethod(IChannelHandler channelHandler, string methodName)
        {

            Ensure.IsNotNull(channelHandler);
            Ensure.IsNotOrEmpty(methodName);

            var _typeName = channelHandler.GetType().FullName;

            if (_direcImplMethods.ContainsKey(_typeName))
            {

                var _methods = _direcImplMethods[_typeName];

                if (_methods == null || _methods.Count() <= 0)
                    return false;

                return _methods.Count(name => name == methodName) > 0;

            }

            return false;
        }

        public void fireChannelRegister()
        {
            this.Head?.fireChannelRegister();
        }

        public void fireChannelActive()
        {
            this.Head?.fireChannelActive();
        }

        public void fireChannelRead(object message)
        {
            this.Head?.fireChannelRead(message);
        }

        public void fireChannelWrite(object message)
        {
            this.Head?.fireChannelWrite(message);
        }

        public void fireChannelClose()
        {
            throw new NotImplementedException();
        }

        public void fireChannelException()
        {
            throw new NotImplementedException();
        }

        public void fireChannelFinally()
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(object message) => this.Head?.WriteAsync(message);

        public Task BindAsync(EndPoint remote) => this.Tail?.BindAsync(remote);

        public Task ConnectAsync(EndPoint remote) => this.Tail?.ConnectAsync(remote);

        public class HeadChannelContext : ChannelHandlerContext
        {
            //public HeadChannelContext()
            //{
            //    this.lifeCycleFlag = LifeCycleFlag.WriteAsync;
            //}

            //是否跳过了指定的方法
            protected override bool IsSkippable(IChannelHandler handler, string methodName)
            {
                return true;
            }

            
        }

        public class TailChannelContext : ChannelHandlerContext
        {
            protected override bool IsSkippable(IChannelHandler handler, string methodName)
            {
                return true;
            }
        }

    }
}
