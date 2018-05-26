using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Buffer
{
    public interface IByteBuf
    {
        /// <summary>
        /// 句柄，在内存池的实现中方便释放
        /// </summary>
        long Handle { get; }

        /// <summary>
        /// 读索引
        /// </summary>
        int ReadIndex { get; }

        /// <summary>
        /// 写索引
        /// </summary>
        int WriteIndex { get; }

        /// <summary>
        /// 当前ByteBuf在byte[]中的偏移
        /// </summary>
        int Offset { get; }

        /// <summary>
        /// 容量
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// 获取缓冲区的字节数组
        /// </summary>
        /// <returns></returns>
        byte[] GetBytes();

        /// <summary>
        /// 是否可读
        /// </summary>
        bool IsReadable();

        /// <summary>
        /// 是否可写
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        bool IsWriteable(int bytes);

        /// <summary>
        /// 写入一个字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IByteBuf WriteByte(byte value);

        /// <summary>
        /// 写入一个Int32的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IByteBuf Write(int value);
        

        /// <summary>
        /// 写入一个字节数组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        IByteBuf Write(byte[] data);

        /// <summary>
        /// 将指定的Byte[]写入到当前的ByteBuf
        /// </summary>
        /// <param name="array">字节数组</param>
        /// <param name="offset">字节数组的内的偏移</param>
        /// <param name="count">写入的长度</param>
        /// <returns>写入成功,返回当前的ByteBuf引用</returns>
        IByteBuf Write(byte[] array, int offset, int count);

        /// <summary>
        /// 将ByteBuf的可写段写入当前的ByteBuf
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        IByteBuf Write(IByteBuf buf);

        /// <summary>
        /// 从当前ReadIndex写入指定长度的buf
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        IByteBuf Write(IByteBuf buf, int length);

        /// <summary>
        /// 读取一个字节
        /// </summary>
        /// <returns></returns>
        Byte ReadByte();

        /// <summary>
        /// 读一个Int32的值
        /// </summary>
        /// <returns></returns>
        int ReadInt32();

        /// <summary>
        /// 获得可读字节的长度
        /// </summary>
        /// <returns></returns>
        int Readables();

        /// <summary>
        /// 获得可写的字节长度
        /// </summary>
        /// <returns></returns>
        int Writeables();

        /// <summary>
        /// 是否可读指定的长度
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        bool CanReadableBytes(int length);

        /// <summary>
        /// readIndex跳过指定的长度
        /// </summary>
        /// <param name="skip"></param>
        IByteBuf ReadSkip(int skip);

        /// <summary>
        /// writeIndex跳过指定的长度
        /// </summary>
        /// <param name="skip"></param>
        /// <returns></returns>
        IByteBuf WriteSkip(int skip);

        /// <summary>
        /// 设置ReadIndex
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IByteBuf SetReadIndex(int index);

        /// <summary>
        /// 设置writeIndex
        /// </summary>
        /// <param name="index"></param>
        IByteBuf SetWriteIndex(int index);

        /// <summary>
        /// 重置writeIndex
        /// </summary>
        IByteBuf ResetWriteIndex();

        /// <summary>
        /// 切片,返回从readindex到length的Bytebuf，不会造成readindex的更新
        /// </summary>
        /// <param name="length"></param>
        /// <param name="creator">提供器,由creator来提供IByteBuf</param>
        /// <returns></returns>
        /// <remarks>
        /// 如果creator=null,那么会new一个SliceBytebuf对象；如果creator!=null,则由creator提供IBytebuf对象
        /// </remarks>
        IByteBuf Slice(int length, IByteBuf slice = null);

        /// <summary>
        /// 清空缓冲区
        /// readIndex=0
        /// writeIndex=0
        /// offtset=0;
        /// capacity=0;
        /// this.bytes=empty
        /// </summary>
        /// <returns></returns>
        IByteBuf Clear();

        /// <summary>
        /// 设置偏移
        /// 
        /// 异常
        /// offset < 0 || offste >= this.bytes.Length,ArgumentException
        /// </summary>
        /// <param name="pageOffset"></param>
        /// <returns></returns>
        IByteBuf SetOffset(int offset);

        /// <summary>
        /// 设置句柄
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        IByteBuf SetHandle(long handle);

        /// <summary>
        /// 设置容量
        /// </summary>
        /// <param name="writeIndex"></param>
        /// <returns></returns>
        IByteBuf SetCapacity(int capacity);

        /// <summary>
        /// 设置字节数组
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        IByteBuf SetBytes(byte[] bytes);

        /// <summary>
        /// 设置字节数组
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="offset">字节数组内的偏移</param>
        /// <param name="capacity">目标buf的容量</param>
        /// <returns></returns>
        IByteBuf SetBytes(byte[] bytes, int offset, int capacity);

        /// <summary>
        /// 是否可写指定的长度
        /// </summary>
        /// <returns></returns>
        bool CanWriteableBytes(int length);

        /// <summary>
        /// 释放缓冲区
        /// </summary>
        void Return();

        
    }
}
