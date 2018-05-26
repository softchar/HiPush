using Hi.Infrastructure.Base;
using Hi.NetWork.Bootstrapping;
using Hi.NetWork.Buffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hi.NetWork.Eventloops
{
    public class MutlEventloopGroup : IEventloopGroup
    {
        public static Func<IEventloop> DefaultNewEventloopFactory = () => new SingleThreadEventloop();

        public static int DefaultEventloopCounter = Environment.ProcessorCount * 2;

        IEventloop[] loops;
        int counter;
        IByteBufAllocator ByteBufAllocator;

        public Func<IEventloop> NewEventloopFactory { get; set; }

        public MutlEventloopGroup()
            : this(DefaultNewEventloopFactory, DefaultEventloopCounter)
        {
            
        }

        public MutlEventloopGroup(int eventloopCount)
            : this(DefaultNewEventloopFactory, eventloopCount)
        {
            
        }

        public MutlEventloopGroup(Func<IEventloop> eventloopFactory, int eventloopCount, IByteBufAllocator bufAlloc = null)
        {
            //Ensure.Ensures(bufAlloc.ArenaCounter < eventloopCount, "IByteBufAllocator.ArenaCounter数量不能小于EventloopCounter");

            this.NewEventloopFactory = eventloopFactory;
            this.ByteBufAllocator = bufAlloc;

            loops = new IEventloop[eventloopCount];
            for (int i = 0; i < eventloopCount; i++)
            {
                loops[i] = NewEventloopFactory();
                if (bufAlloc != null)
                {
                    loops[i].SetAlloc(bufAlloc);
                }
            }
        }

        public IEventloop Next()
        {
            var loop = loops[Math.Abs(counter % loops.Length)];
            Interlocked.Increment(ref counter);
            return loop;
        }

    }
}
