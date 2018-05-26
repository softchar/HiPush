using Hi.Infrastructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Buffer
{
    public class PoolChunkList : HiLinkList<PoolChunk>
    {

        //最小百分比
        byte minPct;

        //最大百分比
        byte maxPct;

        PoolChunkList prev;
        PoolChunkList next;

        public PoolChunkList Prev => prev;
        public PoolChunkList Next => next;

        public PoolChunkList(byte minPct, byte maxPct)
        {
            this.minPct = minPct;
            this.maxPct = maxPct;
        }

        public bool Alloc(out PoolChunk chunk)
        {
            chunk = null;

            if (this.Head == null && this.Tail == null)
                return false;

            chunk = Head;

            return true;
           
        }

        public void SetPrev(PoolChunkList item)
        {
            this.prev = item;
        }

        public void SetNext(PoolChunkList item)
        {
            this.next = item;
        }

        public static void Link(PoolChunkList p1, PoolChunkList p2)
        {
            p1.SetNext(p2);
            p2.SetPrev(p1);
        }


        public bool TryAllocPage(IByteBuf buf, int newSize, int size, out PoolPage page)
        {
            page = null;

            if (Head == Tail && Head == null)
            {
                return false;
            }

            PoolChunk chunk = Head;
            page = chunk.AllocPage(buf, newSize, size);

            while (page == null)
            {
                chunk = chunk.Next;
                if (chunk == null) return false;

                page = chunk.AllocPage(buf, newSize, size);
            }

            //转移chunk
            TryTransfer(chunk);

            return true;
        }

        /// <summary>
        /// 已使用字节数
        /// </summary>
        /// <returns></returns>
        public int Useables()
        {
            int useables = 0;

            var chunklist = this;
            while (chunklist != null)
            {
                var chunk = chunklist.Head;
                while (chunk != null)
                {
                    useables += chunk.Usedables;
                    chunk = chunk.Next;
                }

                chunklist = chunklist.Next;
            }

            return useables;
        }

        /// <summary>
        /// 转移chunk
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="chunk"></param>
        private void TryTransfer(PoolChunk chunk)
        {
            //如果小于最小使用率则将chunk转移到上一个PoolChunkList
            if (chunk.UsedPercent < (minPct / 100f))
            {
                DeleteFirst();

                if (prev == null)
                    return;

                prev.AddLast(chunk);

                return;
               
            }

            //如果大于最大使用率则将chunk转移到下一个PoolChunkList
            if (chunk.UsedPercent >= (maxPct / 100f))
            {
                if (next == null)
                    return;

                next.AddLast(chunk);
                DeleteFirst();

                return;
            }

        }
    }
}
