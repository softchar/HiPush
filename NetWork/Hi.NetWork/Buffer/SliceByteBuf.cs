using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Buffer
{
    /// <summary>
    /// SliceBytebuf只可在指定的范围内读写
    /// </summary>
    public class SliceByteBuf : AbstructByteBuf
    {
        IByteBuf buf;
        IBytebufPool pool;

        public SliceByteBuf()
            :base(0)
        {
        }

        public SliceByteBuf(IBytebufPool pool)
            : this()
        {
            this.pool = pool;
        }

        public SliceByteBuf(IByteBuf buf, int length)
            : this()
        {
            this.buf = buf;
            this.readIndex = buf.ReadIndex;
            this.writeIndex = buf.ReadIndex + length;
            this.SetCapacity(length);
        }

        public override IByteBuf Clear()
        {
            base.Clear();
            this.buf = null;

            return this;
        }

        public override void Return()
        {
            if (pool != null)
            {
                Clear();
                pool.Return(this);
            }
            else
            {
                base.Return();
            }
        }
    }
}
