using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.NetWork.Socketing;
using Hi.NetWork.Buffer;
using Hi.Infrastructure.Base;
using Newtonsoft.Json;

namespace Hi.NetWork.Protocols {

    /// <summary>
    /// 默认协议
    /// </summary>
    public class DefaultProtocol {

        public DefaultProtocol() {

        }

        public virtual HiEvent Serialize(byte[] bytes) {

            Ensure.IsNotNull(bytes);

            var result = new HiEvent();

            string value = Encoding.UTF8.GetString(bytes);

            result = JsonConvert.DeserializeObject<HiEvent>(value);

            return result;
            
        }

        public virtual HiEvent Serialize(ArraySegment<byte> bytes) {

            Ensure.IsNotNull(bytes.Array);

            var result = new HiEvent();

            string value = Encoding.UTF8.GetString(bytes.Array, bytes.Offset, bytes.Count);

            result = JsonConvert.DeserializeObject<HiEvent>(value);

            return result;

        }
    }

    /// <summary>
    /// 业务消息
    /// </summary>
    public class HiEvent {

        /// <summary>
        /// 序号
        /// </summary>
        public string TagId { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public EventType Type { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }

        public HiEvent() {
            TagId = Guid.NewGuid().ToString();
            TimeStamp = DateTime.Now;
        }

    }

    public enum EventType : byte {

        HeathBeat = 0,          //心跳
        Normal = 1,             //默认
        Login = 2,              //登录


    }
}
