using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Eventloops
{
    /// <summary>
    /// 优先级队列（用于存储定时Task的队列，Task越靠前说明离指定的时间越近）
    /// 所以每次只需要取第一个Task的时间与当前的时间对比
    /// </summary>
    public class PriorityQueue<T>
        where T : IComparable<T>
    {
        readonly IComparer<T> comparer;

        //队列的数量
        int count;

        //初始容量
        int capacity;

        //数组
        T[] items;

        public int Count
        {
            get { return count; }
        }

        public PriorityQueue()
            : this(Comparer<T>.Default)
        {

        }

        private PriorityQueue(IComparer<T> comparer)
        {
            this.comparer = comparer;
            this.capacity = 11;
            this.items = new T[this.capacity];
        }

        /// <summary>
        /// 添加一项到队列中
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Enqueue(T item)
        {
            if (count == capacity)
            {
                AdjustContainer();
            }

            items[this.count] = item;
            count++;

            shiftUp(item);
            
            return true;
        }

        /// <summary>
        /// 从队列中取出一个项
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            if (this.count <= 0)
                return default(T);

            //获取第一项
            var first = this.items[0];

            //将第一项和最后一项交换位置
            swap(0, this.count - 1);

            //将最后一项置空,数量减1
            this.items[this.count - 1] = default(T);
            this.count--;

            shiftDown();

            return first;

        }

        /// <summary>
        /// 调整容器,即容量不够时进行扩展,容器空闲太多时进行收缩
        /// </summary>
        private void AdjustContainer()
        {
            capacity += capacity <= 64 ? capacity + 2 : capacity >> 1;
            var newItems = new T[capacity];
            
            //System.Buffer.BlockCopy(this.items, 0, newItems, 0, this.items.Length);
            Array.Copy(this.items, 0, newItems, 0, this.items.Length);
            this.items = newItems;
        }

        /// <summary>
        /// 上升(将index指定的节点与其父节点比较，如果比父节点小，那么与父节点交换位置)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        private void shiftUp(T item)
        {
            int current = this.count - 1;
            while (current > 0)
            {
                //计算父节点的索引
                int parent = (current - 1) >> 1;

                if (this.comparer.Compare(item, items[parent]) >= 0)
                    break;

                var pnode = items[parent];
                swap(current, parent);
                current = parent;
            }
        }

        /// <summary>
        /// 将顶端的节点向下置换（先比较找到两个子节点中较小的那个，如果节点比子节点还要小，那么就互换位置，依次类推）
        /// </summary>
        private void shiftDown()
        {
            int offset = 0;
            while (true)
            {

                int left = (offset << 1) + 1;
                int right = (offset + 1) << 1;

                if (count - 1 < left) break;

                int index =  count - 1 >= right && (comparer.Compare(items[left], items[right]) > 0) ? right : left;
                T value = items[index];
                T vertex = items[offset];


                if (comparer.Compare(value, vertex) < 0)
                {
                    swap(index, offset);
                    offset = index;
                }

            }
        }

        /// <summary>
        /// 交换
        /// </summary>
        private void swap(int index1, int index2)
        {
            var t = this.items[index1];
            this.items[index1] = this.items[index2];
            this.items[index2] = t;
        }
    }
}
