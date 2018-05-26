using Hi.Infrastructure.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Protocols {

    /// <summary>
    /// 消息格式化器
    /// </summary>
    public class MessageFormatter {

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="segment"></param>
        /// <returns></returns>
        public static T Deserializer<T>(ArraySegment<byte> segment) {

            Ensure.IsNotNull(segment.Array);

            var _result = default(T);
            
            var _bytes = System.Text.Encoding.UTF8.GetString(segment.Array, segment.Offset, segment.Count);

            _result = JsonConvert.DeserializeObject<T>(_bytes);

            return _result;

        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static ArraySegment<byte> Serializer(object data) {

            Ensure.IsNotNull(data);

            string _json = JsonConvert.SerializeObject(data);

            var bytes = System.Text.Encoding.UTF8.GetBytes(_json);

            return new ArraySegment<byte>(bytes);

        }

    }
}
