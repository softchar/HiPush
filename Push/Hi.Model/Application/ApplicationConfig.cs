using Hi.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Model.Application
{
    /// <summary>
    /// 项目配置
    /// </summary>
    public class ApplicationConfig : Entity
    {

        /// <summary>
        /// 关联额项目编号
        /// </summary>
        public Guid ProjectToken { get; set; }

        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        protected override void Validate()
        {
            
        }
    }
}
