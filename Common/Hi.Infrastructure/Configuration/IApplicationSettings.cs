using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Configuration
{
    /// <summary>
    /// 定义一个ApplicationSettings接口
    ///     用于可替换实现类
    /// </summary>
    public interface IApplicationSettings
    {
        string LoggerName { get; }

        string ConnectionString { get; }

        T GetSection<T>(string node);

    }
}
