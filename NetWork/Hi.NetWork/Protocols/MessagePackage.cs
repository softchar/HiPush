using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Protocols {

    /// <summary>
    /// 消息包,用于封包和拆包
    /// </summary>
    public class MessagePackage {

        public byte[] Data { get; set; }

        public int Length { get; set; }

        public MessagePackage() { }

        public MessagePackage(int dataLength) {
            Length = dataLength;
            Data = new byte[Length];
        }

    }
}
