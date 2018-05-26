using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hi.NetWork.Eventloops
{

    /// <summary>
    /// 适合多个生产者一个消费者的单向链表队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// 无锁队列
    /// </remarks>
    public class MpscLinkedQueue<T> where T : class
    {
        public readonly static MpscLinkedQueueNode<T> defaultNode = new DefaultNode(null); 

        MpscLinkedQueueNode<T> head;
        MpscLinkedQueueNode<T> tail;

        /// <summary>
        /// 适合多个生产者一个消费者的单向队列
        /// </summary>
        public MpscLinkedQueue()
        {
            head = tail = defaultNode;
        }

        /// <summary>
        /// 讲对象添加到队列中
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool Enqueue(T value)
        {
            var newTail = new MpscLinkedQueueNode<T>() { Value = value, Next = null };

            //F0：1将tail修改为newTail
            var oldTail = GetAndSetTailByCAS(newTail);

            //F1：2将oldTail的Next修改为newTail
            oldTail.Next = newTail;

            return true;
        }

        /// <summary>
        /// 从队列中取出一个项
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 该方法只适合单个消费者
        /// </remarks>
        public virtual T Dequeue()
        {
            var next = PeekNode();

            if (next == null) return null;

            this.head.Next = next.Next;
            
            if (this.tail == next)
            {
                this.tail = this.head;
            }

            return next.Value;
        }

        public bool IsEmpty()
        {
            return head == defaultNode && head == tail;
        }

        private MpscLinkedQueueNode<T> PeekNode()
        {
            var head = this.head;
            var next = head.Next;

            //当运行Enqueue方法的时候,有两个步骤F0和F1;
            //F0是一个原子操作,F1是一个赋值操作;
            //F0和F1的操作都不会造成竞态条件;
            //  但是,有可能会出现一种情况:有两个线程（T1,T2）都在执行F1操作,当T1执行F1后,T2才刚执行F0还没开始执行F1
            //  这个时候就出现了next=null && head != this.tail
            if (next == null && head != this.tail)
            {
                //当出现这种竞态条件时：一般情况是使用锁,
                //但是F0和F1的操作非常短暂，使用锁会浪费资源，所以选择使用自旋锁的方式(类似SpinLock)
                do
                {
                    next = head.Next;
                }
                while (next == null);

            }

            return next;
        }

        /// <summary>
        /// 获取并设置Tail,该操作是原子操作
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private MpscLinkedQueueNode<T> GetAndSetTailByCAS(MpscLinkedQueueNode<T> value)
        {
            return Interlocked.Exchange(ref this.tail, value);
        }

        class DefaultNode : MpscLinkedQueueNode<T>
        {
            public DefaultNode(T value)
            {
                Value = value;
                Next = null;
            }
        }
    }


    /// <summary>
    /// 节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MpscLinkedQueueNode<T>
    {
        public T Value { get; set; }

        public MpscLinkedQueueNode<T> Next { get; set; }

        public MpscLinkedQueueNode()
        {

        }

    }

}
