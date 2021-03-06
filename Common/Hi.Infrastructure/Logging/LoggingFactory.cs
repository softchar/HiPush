﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Logging
{
    /// <summary>
    /// 日志工厂
    /// </summary>
    public class LoggingFactory
    {
        private static ILogger _logger;
        public static void InitializeLogFactory(ILogger logger) {
            _logger = logger;
        }

        public static ILogger GetLogger() {
            return _logger;
        }
    }
}
