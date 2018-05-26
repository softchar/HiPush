using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Messaging
{
    public interface IMessageSender<in TMessage, TResult> 
        where TMessage : IMessage
    {

        /// <summary>
        /// 有返回值的发送
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        TResult Send(TMessage message);

        /// <summary>
        /// 无返回值的异步发送
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<TResult> SendAsync(TMessage message);
    }
}
