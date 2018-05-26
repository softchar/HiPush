using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Parameters;

namespace Hi.Infrastructure.Ioc
{
    public class NInjectContainer : IContainer
    {
        public IKernel kernel;
        public NInjectContainer() {
            kernel = new StandardKernel();
        }


        public T Get<T>()
        {
            try
            {
                return kernel.Get<T>();
            }
            catch
            {
                return default(T);
            }
        }

        public object Get(Type service) {
            return kernel.Get(service);
        }

        public void Register<TFrom, TTo>() where TTo : TFrom
        {
            kernel.Bind<TFrom>().To<TTo>();
        }

        public void RegisterInstance<TFrom>(){
            kernel.Bind<TFrom>().ToSelf().InSingletonScope();
        }

        public void RegisterInstance<TFrom, TTo>() where TTo : TFrom
        {
            kernel.Bind<TFrom>().To<TTo>().InSingletonScope();
        }

        public T TryGet<T>(Type type)
        {
            try
            {
                return (T)kernel.TryGet(type);
            }
            catch
            {
                return default(T);
            }
        }
    }
}
