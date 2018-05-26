using Hi.Infrastructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Buffer
{
    public class AbstructByteBuf : IByteBuf
    {
        //默认容量
        public static byte[] Emtpy = new byte[0];
        
        public readonly static int TINY = 1 << 2;        //4;
        public readonly static int SMALL = 1 << 8;       //256;
        public readonly static int MEDIUM = 1 << 10;     //1024;
        public readonly static int NORMAL = 1 << 12;     //4096;
        public readonly static int BIG = 1 << 13;        //8192;
        public readonly static int HUGE = 1 << 14;       //16K=16384
        public readonly static int K32 = 1 << 15;        //32K=32768
        public readonly static int K64 = 1 << 16;        //64K=65535

        /// <summary>
        /// 4个字节
        /// </summary>
        /// <returns></returns>
        public static IByteBuf Tiny() => new AbstructByteBuf(TINY);

        /// <summary>
        /// 256个字节
        /// </summary>
        /// <returns></returns>
        public static IByteBuf Small() => new AbstructByteBuf(SMALL);

        /// <summary>
        /// 1024个字节
        /// </summary>
        /// <returns></returns>
        public static IByteBuf Medium() => new AbstructByteBuf(MEDIUM);

        /// <summary>
        /// 4096个字节
        /// </summary>
        /// <returns></returns>
        public static IByteBuf Normal() => new AbstructByteBuf(NORMAL);

        /// <summary>
        /// 8192个字节
        /// </summary>
        /// <returns></returns>
        public static IByteBuf Big() => new AbstructByteBuf(BIG);

        /// <summary>
        /// 16K个字节
        /// </summary>
        /// <returns></returns>
        public static IByteBuf Huge() => new AbstructByteBuf(HUGE);

        /// <summary>
        /// 32K个字节
        /// </summary>
        /// <returns></returns>
        public static IByteBuf K_32() => new AbstructByteBuf(K32);

        /// <summary>
        /// 64K个字节
        /// </summary>
        /// <returns></returns>
        public static IByteBuf K_64() => new AbstructByteBuf(K32);

        protected int readIndex;
        protected int writeIndex;
        private byte[] bytes;
        private int offset;
        private int capacity;
        private long handle;

        //Int32位所占字节长度
        private byte INT32SIZE = sizeof(Int32);

        //Byte所占的字节长度
        private byte BYTESIZE = sizeof(byte);

        /// <summary>
        /// 读索引
        /// </summary>
        public int ReadIndex => readIndex;

        /// <summary>
        /// 写索引
        /// </summary>
        public int WriteIndex => writeIndex;

        /// <summary>
        /// 容量
        /// </summary>
        public int Capacity => capacity;

        /// <summary>
        /// 字节数组
        /// </summary>
        public byte[] Bytes => bytes;

        /// <summary>
        /// 偏移
        /// </summary>
        public int Offset => offset;

        /// <summary>
        /// 最大索引,索引范围[offset, maxIndex);
        /// 比如：
        /// offset=0,capacity=3,索引范围[0,3),maxIndex=3,实际可以使用的是[0, 1, 2] 
        /// offset=1,capacity=3,索引范围[1,4),maxIndex=4
        /// maxIndex=offset+capacity
        /// offset,readIndex,writeIndex,capacity都不能大于等于maxIndex
        /// </summary>
        public int MaxIndex => capacity + offset;

        /// <summary>
        /// 句柄，在雷存池的视线中方便释放
        /// </summary>
        public long Handle => handle;

        /// <summary>
        /// 是否可读
        /// </summary>
        public bool IsReadable() => writeIndex > ReadIndex;

        /// <summary>
        /// 是否可写
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public bool IsWriteable(int bytes) => MaxIndex - writeIndex >= bytes;

        public AbstructByteBuf()
        {
            bytes = Emtpy;
        }

        public AbstructByteBuf(int capacity)
        {
            this.capacity = capacity;
            bytes = new byte[capacity];
        }

        public IByteBuf WriteByte(byte value)
        {
            EnsureWriteIndexValid(writeIndex + 1);

            bytes[writeIndex] = value;

            writeIndex++;

            return this;
        }

        /// <summary>
        /// 写一个int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public unsafe IByteBuf Write(int value)
        {

            EnsureWriteIndexValid(writeIndex + INT32SIZE);

            fixed (byte* b = bytes)
            {
                *((int*)(b + writeIndex)) = value;
            }

            writeIndex += INT32SIZE;

            return this;
        }

        /// <summary>
        /// 将data数组写入buf中
        /// 
        /// 异常
        /// array == null 抛出ArgumentNullException
        /// array.Length大于可写长度时 抛出IndexOutOfRangeException
        /// </summary>
        /// <param name="array">字节数组</param>
        /// <returns></returns>
        public IByteBuf Write(byte[] array)
        {
            BlockCopy(array, 0, array.Length);
            return this;
        }

        /// <summary>
        /// 将data数组写入buf中
        /// 
        /// 异常
        /// array == null 抛出ArgumentNullException
        /// offset和count索引有问题时 抛出IndexOutOfRangeException
        /// </summary>
        /// <param name="array">字节数组</param>
        /// <param name="offset">字节数组内的偏移</param>
        /// <param name="count">需要写入的长度</param>
        /// <returns></returns>
        public IByteBuf Write(byte[] array, int offset, int count)
        {
            BlockCopy(array, offset, count);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void BlockCopy(byte[] array, int offset, int count)
        {
            EnsureWriteIndexValid(writeIndex + count);

            System.Buffer.BlockCopy(array, offset, this.bytes, writeIndex, count);

            this.writeIndex += count;
        }

        /// <summary>
        /// 摘要
        /// 
        /// 将buf写入当前的Bytebuf缓冲区中
        /// 
        /// 
        /// 异常
        /// buf == null 抛出ArgumentNullException
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        public IByteBuf Write(IByteBuf buf)
        {
            Ensure.IsNotNull(buf, $"Write(buf:{buf}) buf不能为空");

            //将buf的数据复制到当前的数据中
            BlockCopy(buf.GetBytes(), 0, buf.Readables());

            return this;
        }

        public IByteBuf Write(IByteBuf buf, int length)
        {
            Ensure.IsNotNull(buf, $"Write(IByteBuf:{buf},length:{length}) buf不能为null");

            BlockCopy(buf.GetBytes(), buf.ReadIndex, length);

            return this;

        }

        /// <summary>
        /// 读一个int
        /// </summary>
        /// <returns></returns>
        public unsafe int ReadInt32()
        {
            //TODO
            //验证是否可以读一个Int32类型的值
            EnsureReadIndexValid(readIndex + INT32SIZE);

            int value = 0;

            fixed (byte* pbyte = &bytes[readIndex])
            {
                //讲byte*转换成int*再取值
                value = *((int*)pbyte);

                //小端模式
                //return (*pbyte) | (*(pbyte + 1) << 8) | (*(pbyte + 2) << 16) | (*(pbyte + 3) << 24);

                //大端模式
                //return (*pbyte << 24) | (*(pbyte + 1) << 16) | (*(pbyte + 2) << 8) | (*(pbyte + 3));
            }

            readIndex += INT32SIZE;

            return value;

        }

        public byte ReadByte()
        {
            EnsureReadIndexValid(readIndex + 1);

            byte result = bytes[readIndex];

            readIndex++;

            return result;
        }

        /// <summary>
        /// readIndex跳过指定的长度
        /// </summary>
        /// <param name="skip"></param>
        public IByteBuf ReadSkip(int skip)
        {
            EnsureReadIndexValid(readIndex + skip);

            readIndex += skip;

            return this;
        }

        /// <summary>
        /// writeIndex跳过指定的长度
        /// </summary>
        /// <param name="skip"></param>
        public IByteBuf WriteSkip(int skip)
        {
            EnsureWriteIndexValid(writeIndex + skip);

            writeIndex += skip;

            return this;
        }

        /// <summary>
        /// 设置ReadIndex
        /// </summary>
        /// <param name="index"></param>
        public IByteBuf SetReadIndex(int index)
        {
            EnsureReadIndexValid(index);

            readIndex = index;

            return this;
        }

        /// <summary>
        /// 重置ReadIndex
        /// </summary>
        public IByteBuf ResetReadIndex()
        {
            readIndex = offset;

            return this;
        }

        /// <summary>
        /// 设置writeIndex
        /// </summary>
        /// <param name="index"></param>
        public IByteBuf SetWriteIndex(int index)
        {
            EnsureWriteIndexValid(index);

            writeIndex = index;

            return this;
        }

        /// <summary>
        /// 重置writeIndex
        /// </summary>
        public IByteBuf ResetWriteIndex()
        {
            writeIndex = offset;

            return this;
        }

        /// <summary>
        /// 确认读索引是否有效
        /// </summary>
        public void EnsureReadIndexValid(int newIndex)
        {
            if (newIndex < 0 || newIndex > writeIndex || newIndex > MaxIndex)
            {
                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// 确认写索引是否有效
        /// </summary>
        /// <param name="newIndex"></param>
        public void EnsureWriteIndexValid(int newIndex)
        {
            if (newIndex < 0 || newIndex < readIndex || newIndex > MaxIndex)
            {
                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// 清空ByteBuffer，仅仅是重置readIndex和writeIndex
        /// </summary>
        public virtual IByteBuf Clear()
        {
            readIndex = offset;
            writeIndex = offset;
            return this;
        }

        /// <summary>
        /// 获取字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return bytes;
        }

        /// <summary>
        /// 获得内容的长度
        /// </summary>
        /// <returns></returns>
        public int Readables()
        {
            return writeIndex - readIndex;
        }

        public bool CanReadableBytes(int length)
        {
            return Readables() >= length;
        }

        /// <summary>
        /// 获得可写的长度
        /// </summary>
        /// <returns></returns>
        public int Writeables()
        {
            return MaxIndex - writeIndex;
        }

        public bool CanWriteableBytes(int length)
        {
            return Writeables() >= length;
        }

        /// <summary>
        /// bytebuf分片，会更新当前的bytebuf的readindex
        /// </summary>
        /// <param name="length"></param>
        /// <param name="slice"></param>
        /// <returns></returns>
        public IByteBuf Slice(int length, IByteBuf slice = null)
        {
            var slice0 = slice != null ? slice : new SliceByteBuf(this, length);

            //这里需要注意顺序 capacity -> writeindex -> readindex
            slice0.SetCapacity(this.ReadIndex + length);
            slice0.SetWriteIndex(this.ReadIndex + length);
            slice0.SetReadIndex(this.ReadIndex);
            slice0.SetBytes(this.bytes);

            this.ReadSkip(length);

            return slice0;
        }

        public IByteBuf SetCapacity(int capacity)
        {
            this.capacity = capacity;
            return this;
        }

        /// <summary>
        /// 设置字节缓冲区
        /// 
        /// 异常
        /// 当bytes=null时,抛出异常ArgumentNullException
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        public IByteBuf SetBytes(byte[] bytes)
        {
            Ensure.Assert(bytes == null, $"SetBytes(bytes:{bytes}) Exception bytes不能为空");

            this.bytes = bytes;
            this.offset = 0;
            this.readIndex = 0;
            this.writeIndex = 0;
            this.capacity = bytes.Length;

            return this;
        }

        /// <summary>
        /// 设置字节缓冲区
        /// 
        /// 异常：
        /// bytes=null,抛出异常Exception
        /// offset < 0 || offset >= bytes.Length,抛出异常Exception
        /// offset + capacity > bytes.Length,抛出异常Exception
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="offset">字节数组的偏移</param>
        /// <param name="capacity">需要设置的长度</param>
        /// <returns></returns>
        public IByteBuf SetBytes(byte[] bytes, int offset, int capacity)
        {
            Ensure.Assert(bytes == null, "bytes不能为空");
            Ensure.Assert(offset < 0 || offset >= bytes.Length, "offset超出索引范围");
            Ensure.Assert(offset + capacity > bytes.Length);

            this.bytes = bytes;
            this.offset = offset;
            this.readIndex = offset;
            this.writeIndex = offset;
            this.capacity = capacity;

            return this;

        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Return()
        {
            this.offset = 0;
            Clear();
            this.capacity = 0;
            this.bytes = Emtpy;
        }

        /// <summary>
        /// 设置偏移
        /// 
        /// 异常
        /// offset < 0 || offste >= this.bytes.Length,ArgumentException
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public IByteBuf SetOffset(int offset)
        {
            Ensure.Assert(offset < 0 || offset >= this.bytes.Length, $"SetOffset(offset:{offset}) Exception, offset不能大于或者等于this.Bytes.Length");

            this.offset = offset;
            this.readIndex = offset;
            this.writeIndex = offset;

            //重新计算容量
            this.capacity = this.bytes.Length - offset;

            return this;
        }

        public IByteBuf SetHandle(long handle)
        {
            this.handle = handle;
            return this;
        }

        
    }
}
