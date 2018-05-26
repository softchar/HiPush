using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Infrastructure.Ioc;

namespace Hi.Infrastructure.Messaging
{
    public class HandleRegisterEntryBase
    {

        public HandleRegisterEntryBase()
        {
            RegisterHandler();
        }

        public void RegisterHandler()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                //找到实现类
                foreach (var handle in assembly.GetTypes().Where(x => IsImplIHandler(x, HandlerType())))
                {
                    var handler = CreateInstance(handle);

                    //找到实现的接口
                    foreach (var handlerInterfaceType in handle.GetInterfaces().Where(x => IsGenericIHandler(x, HandlerType())))
                    {
                        //实现接口的第一个参数
                        var eventDataType = handlerInterfaceType.GetGenericArguments().First();
                        RegisterHandler(handler, eventDataType);
                    }
                }
            }
        }

        private void RegisterHandler(object handler, Type eventDataType)
        {
            var registerHandlerMethod = this.GetType().GetMethods().Single
            (
                m => m.Name == "RegisterHandler" && m.IsGenericMethod && m.GetParameters().Count() == 1
            );

            var targetMethod = registerHandlerMethod.MakeGenericMethod(new[] { eventDataType });
            targetMethod.Invoke(this, new object[] { handler });

        }

        private bool IsImplIHandler(Type type, Type handlerType)
        {
            return type.IsClass && !type.IsAbstract && type.GetInterfaces().Any(x => x.IsGenericType && (x.GetGenericTypeDefinition() == handlerType));
        }

        private bool IsGenericIHandler(Type type, Type handlerType)
        {
            return type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == handlerType;
        }

        public virtual Type HandlerType()
        {
            throw new NotImplementedException();
        }

        public object CreateInstance(Type type) {

            var constructors = type.GetConstructors();

            foreach (var constructor in constructors)
            {
                var constructorParmas = constructor.GetParameters();

                List<object> services = new List<object>();

                foreach (var paramType in constructorParmas )
                {
                    var obj = InterFaceFactory.Container.Get(paramType.ParameterType);
                    if (obj != null) {
                        services.Add(obj);
                    }
                }

                if (constructorParmas.Count() == services.Count()) {
                    return Activator.CreateInstance(type, services.ToArray());
                }
                
            }

            return Activator.CreateInstance(type);
        }
    }
}
