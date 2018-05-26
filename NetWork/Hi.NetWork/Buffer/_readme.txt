


+Buffer缓冲区
说明：通信过程中，所有的数据交换都经过该缓冲区，且只有这一个缓冲区。
缓冲区分为通道缓冲区（ChannelByteBuffer）和业务数据缓冲区（ByteBuffer）。

增长策略：每次增加一块内存块（长度为：ByteIncrement，由配置文件配置）
当这块内存全部被释放之后，将这块内存回收掉，并重新整理整块内存


ChannelByteBuffer：通道缓冲区
说明：每一个通道都固定会有两个标准缓冲区（接收缓冲区，发送缓冲区），大小不变。每激活一个Channel，分配两个固定不变的buffer。
在通信过程中，会一直接收数据至缓冲区中，待缓冲区没有足够的空间接收下一次数据时，通道状态更新为等待状态（等待缓冲区被释放出足够的地址空间，才开始接收下一次的数据）。

ByteBuffer：
说明：Buffer中除了ChannelByteBuffer之外的所有的地址空间就是一整个ByteBuffer，用于将




/*********************************************************************************************************
接收缓冲区：固定缓存大小

每个Channel默认占4096个字节长度的接收缓冲区，一个接收缓冲区是一个IByteBuf接口。

/*********************************************************************************************************

*Bootstrap(引导)
	
*ThreadEventloopGroup（事件循环组）
	EventloopGroup包含有一个指定个数的的Eventloop数组
	EventloopGroup将Channel负载均衡分配到Eventloop中，其实是发布ChannelRegisterToEventloop事件


*ChannelRegisterToEventloop事件：将Channel注册到Eventloop，Eventloop.RegisterChannel(channel)
	RegisterChannel中将PooledByteBuf的引用绑定到Channel


*ChannelActiveted事件：Channel已激活
	给Channel分配ByteBuf，即PooledByteBuf.Get()获得一块内存缓冲区用作读缓冲区

	