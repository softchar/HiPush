using Hi.Infrastructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hi.NetWork.Buffer
{
    public abstract class HiThreadLocal<T>
        where T : HiThreadLocal<T> , new()
    {
        //表示这个类型占用的index
        static int index = ThreadLocalMap.GetNextAvailableIndex();

        public HiThreadLocal()
        {
            
        }

        T Initialize0()
        {
            var value = Initialize();
            Set(ThreadLocalMap.GetMap(), index, value);
            return value;
        }

        //初始化
        protected abstract T Initialize();

        public static T Value => Get(ThreadLocalMap.GetMap(), index);

        /// <summary>
        /// 当Index为负数时，会抛出IndexOutOfException异常
        /// </summary>
        /// <param name="map"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static T Get(ThreadLocalMap map, int index)
        {
            object val = map.Get(index);
            if (val != null)
            {
                return (T)val;
            }

            return NewObjectFactory().Initialize0();
        }

        public static Func<T> NewObjectFactory = () => new T();

        protected static T Get()
        {
            return Get(ThreadLocalMap.GetMap(), index);
        }

        /// <summary>
        /// 当Index为负数时，会抛出IndexOutOfException异常
        /// </summary>
        /// <param name="map"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void Set(ThreadLocalMap map, int index, T value)
        {
            map.Set(index, value);
        }

    }
}
