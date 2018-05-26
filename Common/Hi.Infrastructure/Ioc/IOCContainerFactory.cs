using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Ioc
{
    public class IOCContainerFactory
    {
        
        public static IContainer GetContainer() {
            //这里先固定使用NInject
            IContainer ioc = new NInjectContainer();
            return ioc;
        }
    }
}
