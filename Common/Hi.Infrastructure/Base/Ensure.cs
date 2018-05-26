using System;
using System.Linq;


namespace Hi.Infrastructure.Base
{
    public static class Ensure
    {

        /// <summary>
        /// 确保对象不为Null,如果为Null则抛出ArgumentNullException异常
        /// </summary>
        /// <param name="bytes"></param>
        public static void IsNotNull(byte[] bytes, string message = null)
        {

            if (bytes == null || bytes.Count() == 0)
            {
                throw new ArgumentNullException(message);
            }

        }

        /// <summary>
        /// 确保对象不为Null,如果为Null则抛出ArgumentNullException异常
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        public static void IsNotNull(object obj, string message = null)
        {

            if (obj == null)
            {
                throw new ArgumentNullException(message);
            }

        }

        public static void IsNotNull(object[] objs, string message = null)
        {

            if (objs == null) throw new ArgumentNullException(message);

            if (objs.Count(item => item == null) > 0) throw new ArgumentNullException(message);

        }

        /// <summary>
        /// 确保字符串不为Null或Empth,如果为Null则抛出ArgumentNullException异常
        /// </summary>
        /// <param name="str"></param>
        /// <param name="message"></param>
        public static void IsNotOrEmpty(string str, string message = null)
        {

            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException(message);
            }


        }

        /// <summary>
        /// 如果字符串location=null就替换成value
        /// </summary>
        /// <param name="location"></param>
        /// <param name="change"></param>
        public static void CompareExchange(ref string location, string value)
        {

            location = !string.IsNullOrEmpty(location) ? location : value;

        }

        public static void CompareExchange<T>(ref T location, T value)
        {
            if (location == null)
                location = value;
        }

        /// <summary>
        /// 数字
        /// </summary>
        /// <param name="location"></param>
        /// <param name="value"></param>
        /// <param name="comparev"></param>
        public static void CompareExchange(ref int location, int value, int comparev)
        {

            //System.Threading.Interlocked.CompareExchange(ref location, value, comparev);
            location = location != comparev ? location : value;

        }

        /// <summary>
        /// 当condition=false时抛出异常
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        public static void Ensures(bool condition, string message = null)
        {

            if (!condition)
            {
                throw new ArgumentException(message);

            }

        }

        /// <summary>
        /// 当condition=true时，抛出异常
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        public static void Assert(bool condition, string message = null)
        {

            if (condition)
            {
                throw new ArgumentException(message);
            }

        }
    }
}
