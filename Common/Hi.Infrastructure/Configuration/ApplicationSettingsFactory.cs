using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Configuration
{
    /// <summary>
    /// ApplicationSettings工厂
    /// </summary>
    public class ApplicationSettingsFactory
    {
        private static IApplicationSettings _applicationSettings;

        /// <summary>
        /// 全局初始化
        /// </summary>
        /// <param name="applicationSettings"></param>
        public static void InitialzeApplicationSettingsFactory(IApplicationSettings applicationSettings) {
            _applicationSettings = applicationSettings;
        }

        /// <summary>
        /// 获取初始化后的IApplicationSettings
        /// </summary>
        /// <returns></returns>
        public static IApplicationSettings GetApplicationSettings() {
            return _applicationSettings;
        }
    }
}
