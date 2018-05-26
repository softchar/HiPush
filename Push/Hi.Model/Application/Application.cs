
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Application
{
    using Hi.Infrastructure.Domain;
    using Hi.Model.Messaging;
    using BusinessRules;
    public class Application : Entity, IAggregateRoot
    {

        /// <summary>
        /// AppId
        /// </summary>
        public Guid AppId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 消息列表
        /// </summary>
        public virtual ICollection<Message> Messages { get; set; }

        /// <summary>
        /// 项目配置
        /// </summary>
        private List<ApplicationConfig> config ;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Application() {

            AppId = Guid.NewGuid();

            Messages = new HashSet<Message>();

        }

        /// <summary>
        /// 添加配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddConfigSetting(string key, string value) {

            if (config == null) config = new List<ApplicationConfig>();

            config.Add(new ApplicationConfig() { ProjectToken = this.Token, Key = key, Value = value });

        }

        /// <summary>
        /// 验证业务规则
        /// </summary>
        protected override void Validate() {

            if (Guid.Empty == AppId) AddBrokenRule(ApplicationBusinessRule.AppIdIsNull);

            if (string.IsNullOrEmpty(AppName)) AddBrokenRule(ApplicationBusinessRule.AppNameIsHad);

        }
    }
}
