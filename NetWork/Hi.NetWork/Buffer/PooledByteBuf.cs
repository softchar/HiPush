using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Buffer
{
    /// <summary>
    /// 从池内获得的Buf
    /// </summary>
    public class PooledByteBuf : AbstructByteBuf
    {
        IBytebufPool pool;

        public PooledByteBuf()
        {

        }

        public PooledByteBuf(IBytebufPool pool)
        {
            this.pool = pool;
        }

        public override void Return()
        {
            base.Return();

            //这里需要从池内释放
            pool?.Return(this);
        }
    }
}
