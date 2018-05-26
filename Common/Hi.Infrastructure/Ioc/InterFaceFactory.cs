using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Parameters;

namespace Hi.Infrastructure.Ioc
{
    
    public class InterFaceFactory
    {
        public static IContainer Container = IOCContainerFactory.GetContainer();

    }
}
