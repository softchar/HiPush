using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Buffer
{
    public interface IByteBufAllocator
    {
        IByteBuf Buffer(int size);
    }

    public class DefaultByteBufAllocator : IByteBufAllocator
    {
        public static DefaultByteBufAllocator Default;
        public static IByteBuf Empty = new FixedLengthByteBuf();

        static DefaultByteBufAllocator()
        {
            Default = new DefaultByteBufAllocator();
        }

        public IByteBuf Buffer(int size)
        {
            return new FixedLengthByteBuf(size);
        }
    }

}
