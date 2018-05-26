using Hi.NetWork.Buffer;
using Hi.NetWork.Socketing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Protocols {

    /// <summary>
    /// 协议接口
    /// </summary>
    public interface IFramer {

        /// <summary>
        /// 打包
        /// </summary>
        IEnumerable<ArraySegment<byte>> Packet(ArraySegment<byte> data);

        /// <summary>
        /// 解包
        /// </summary>
        //void Unpacking(IEnumerable<ArraySegment<byte>> segment);
        void Unpacking(IEnumerable<ArraySegment<byte>> segment);

        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="block"></param>
        void Unpacking(ArraySegment<byte> block);

        /// <summary>
        /// 包解析完成事件
        /// </summary>
        Action<ArraySegment<byte>> UnPacketedCompleted { get; set; }
    }
}
