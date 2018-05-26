using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hi.Model.Messaging
{
    using BusinessRules;
    using Hi.Infrastructure.Domain;
    using Enum;

    /// <summary>
    /// 消息
    /// </summary>
    public class Message : Entity, IAggregateRoot
    {

        /// <summary>
        /// AppId
        /// </summary>
        public Guid AppId { get; set; }

        /// <summary>
        /// BodyToken
        /// </summary>
        public System.Guid BodyToken { get; set; }

        /// <summary>
        /// 消息发送的目标设备类型
        /// </summary>
        public MessageDeviceType DeviceType { get; set; }

        /// <summary>
        /// 消息体
        /// </summary>
        public virtual MessageBody Body { get; private set; }

        /// <summary>
        /// 消息所在的应用程序
        /// </summary>
        public virtual Application.Application Application { get; set; }

        /// <summary>
        /// 设置/修改消息体
        /// </summary>
        /// <param name="body"></param>
        public void SetBody(MessageBody body) {
            Body = body;
            BodyToken = body.Token;
        }

        protected override void Validate()
        {
            if (Body == null) {
                AddBrokenRule(MessageBusinessRule.BodyIsNull);
            }
            var brokenRules = Body.GetBrokenRules();
            if (brokenRules.Count() > 0) {
                foreach (var rules in brokenRules)
                {
                    AddBrokenRule(rules);
                }
            }
        }

    }
}