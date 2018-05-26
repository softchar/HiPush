using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Ioc
{
    public interface IContainer
    {
        void Register<TFrom, TTo>() where TTo : TFrom;
        void RegisterInstance<TFrom>();
        void RegisterInstance<TFrom, TTo>() where TTo : TFrom;

        T Get<T>();
        object Get(Type service);
        T TryGet<T>(Type type);
    }
}
