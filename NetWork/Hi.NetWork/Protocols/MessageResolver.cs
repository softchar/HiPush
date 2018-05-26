using Hi.Infrastructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Protocols {

    /// <summary>
    /// 包解析器
    /// </summary>
    /// <remarks>
    /// 包的构建和解析为用户业务数据
    /// </remarks>
    public class MessageResolver {

        private const int _headerSize = sizeof(Int32);

        /// <summary>
        /// 构建MessagePackage对象
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static MessagePackage BuildPack(string str) {

            Ensure.IsNotOrEmpty(str);

            var __buffer = Encoding.UTF8.GetBytes(str);

            return BuildPack(__buffer);
        }

        public static MessagePackage BuildPack(ArraySegment<byte> segment) {

            Ensure.IsNotNull(segment.Array);

            var __buffer = segment.Array;

            return BuildPack(__buffer);
        }

        public static MessagePackage BuildPack(byte[] buffer) {

            Ensure.IsNotNull(buffer);

            var pack = new MessagePackage(_headerSize + buffer.Length);

            for (int i = 0; i < _headerSize; i++) {
                pack.Data[i] |= (byte)(buffer.Length >> i * 8);
            }

            System.Buffer.BlockCopy(buffer, 0, pack.Data, _headerSize, buffer.Length);

            return pack;

        } 

        /// <summary>
        /// 将数据解析为MessagePackage对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MessagePackage ResolvePack(byte[] data) {

            Ensure.IsNotNull(data, "data不能为空");
            Ensure.Assert(data.Length <= _headerSize, "数据长度应该大于4");

            var pack = new MessagePackage(data.Length - _headerSize);

            System.Buffer.BlockCopy(data, _headerSize, pack.Data, 0, pack.Length);

            return pack;
        }
    }
}
