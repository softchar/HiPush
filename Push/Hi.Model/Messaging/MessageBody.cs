using Hi.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Messaging
{
    using BusinessRules;
    /// <summary>
    /// 消息体
    /// </summary>
    public class MessageBody : Entity
    {
        public string Title { get; set; }
        public string Content { get; set; }

        protected override void Validate()
        {
            if (string.IsNullOrEmpty(Title) || Title.Length > 50)
            {
                AddBrokenRule(MessageBusinessRule.TitleIsNullOrEmpty);
            }
           
            if (string.IsNullOrEmpty(Content) || Content.Length > 200)
            {
                AddBrokenRule(MessageBusinessRule.ContentIsNullOrEmpty);
            }
        }
    }
}
