using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hi.NetWork.Buffer
{
    /// <summary>
    /// 本地线程缓存表
    /// </summary>
    public class ThreadLocalMap
    {
        public static readonly int EmptyIndex = -1;
        static readonly int DefaultVariableCounter = 32;
        static readonly int DefaultMaxVariableCounter = 256;
        static readonly object empty = null;
        static int nextIndex = EmptyIndex;

        [ThreadStatic]
        static ThreadLocalMap map;

        object[] variables;

        int capacity;

        //将该方法编译为内联函数
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ThreadLocalMap GetMap()
        {
            if (map == null)
            {
                map = new ThreadLocalMap(DefaultVariableCounter);
            }
            return map;
        }

        /// <summary>
        /// 获得下一个索引
        /// </summary>
        /// <returns>-1表示失败，非-1表示索引</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetNextAvailableIndex()
        {
            return Interlocked.Increment(ref nextIndex);
        }

        public object Get(int index)
        {
            if (index == EmptyIndex)
                return null;
            return variables[index];
        }

        public void Set(int index, object obj)
        {
            if (index >= DefaultMaxVariableCounter)
            {
                throw new IndexOutOfRangeException($"index最大值为{DefaultMaxVariableCounter},index:{index}");
            }

            if (index >= capacity)
            {
                capacity <<= 1;
                Array.Resize(ref variables, capacity);
            }
            variables[index] = obj;
        }

        public ThreadLocalMap(int variableCapacity)
        {
            this.capacity = variableCapacity;
            variables = new object[variableCapacity];

            for (int i = 0; i < variableCapacity; i++)
            {
                variables[i] = empty;
            }
        }
    }
}
