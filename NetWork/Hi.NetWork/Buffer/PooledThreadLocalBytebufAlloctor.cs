using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hi.NetWork.Buffer
{
    public class PooledThreadLocalBytebufAlloctor : HiThreadLocal<PooledThreadLocalBytebufAlloctor> , IByteBufAllocator
    {
        static int DefaultArenaCounter = Environment.ProcessorCount * 2;
        static int DefaultChunkCounter = 25;

        PoolArena arena;
        int chunkCounter;

        public PooledThreadLocalBytebufAlloctor()
            : this(DefaultChunkCounter)
        {
        }

        public PooledThreadLocalBytebufAlloctor(int chunkCounter)
        {
            this.chunkCounter = chunkCounter;
        }

        public IByteBuf Buffer(int size)
        {
            return arena.Alloc(size);
        }

        protected override PooledThreadLocalBytebufAlloctor Initialize()
        {
            arena = new PoolArena(chunkCounter);
            return this;
        }
    }
}
