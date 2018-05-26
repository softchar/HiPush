using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.Infrastructure.Base;

namespace Hi.NetWork.Buffer
{
    /// <summary>
    /// 固定长度的字节缓冲区
    /// </summary>
    public class FixedLengthByteBuf : AbstructByteBuf
    {
        IBytebufPool pool;
        public FixedLengthByteBuf()
        {

        }

        public FixedLengthByteBuf(IBytebufPool pool)
            : this()
        {
            this.pool = pool;
        }

        public FixedLengthByteBuf(int capacity, IBytebufPool pool = null)
            : base(capacity)
        {
            this.pool = pool;
        }

        public override void Return()
        {
            Clear();
            if (pool != null)
            {
                pool.Return(this);
            }
            else
            {
                base.Return();
            }
        }

    }

}
