using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Ioc
{
    using Ninject;
    public class IInterFaceFactory
    {
        public static IKernel Kernel = new StandardKernel();

        public static T Get<T>() {
            return Kernel.Get<T>();
        }
    }
}
