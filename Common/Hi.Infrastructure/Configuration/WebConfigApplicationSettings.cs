using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Configuration {

    /// <summary>
    /// 基于WebConfig的ApplicationSetting实现
    /// </summary>
    public class WebConfigApplicationSettings : IApplicationSettings {

        public string ConnectionString {
            get {
                return ConfigurationManager.ConnectionStrings["ConStr"].ConnectionString;
            }
        }

        public string LoggerName {
            get {
                return ConfigurationManager.AppSettings["LoggerName"];
            }
        }

        public virtual T GetSection<T>(string node) {
            return default(T);
        }
    }
}
