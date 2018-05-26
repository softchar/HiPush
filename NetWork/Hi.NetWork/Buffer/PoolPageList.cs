using Hi.Infrastructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Buffer
{
    /// <summary>
    /// PoolPage集合，链表
    /// </summary>
    public class PoolPageList : HiLinkList<PoolPage>
    {

        //元素的尺寸
        int elemSize;

        public int ElemSize
        {
            get { return elemSize; }
        }

        public PoolPageList(int eleSize)
        {
            this.elemSize = eleSize;

        }

        /// <summary>
        /// 获得下一个可用的Page，如果找不到则返回null
        /// </summary>
        /// <returns></returns>
        public PoolPage GetNextAvailPage()
        {
            var page = Head;

            while (page != null && !page.CanAlloc)
            {
                page = page.Next;
            }

            return page;

        }
    }
}
