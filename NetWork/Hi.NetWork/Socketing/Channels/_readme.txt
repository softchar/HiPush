


+Socketing套接字

	服务端:
		NetWorker: 网络工作者
		ServerChannel: 服务端通道,负责监听连接,断开,重连
		Session: 会话管理
		Acceptor：接收器
			用于处理监听连接，分配通道
		

	客户端:
		ClientChannel: 客户端通道,负责连接，断开


	通用：
		通道：
			1，一个Channel对象对应一个Pipeline，关系1v1
			2，一个Channel对应一个Context，关系1v1
			3，一个Channel对应一个Invoker，关系1v1
			4，一个Channel对应一个Eventloop，但是一个Eventloop可以绑定到多个Channel，关系1vN

			Channel：IO读写通道
			ChannelInput：通道输入流
			ChannelOutput：通道输出流
			ChannelWriter：通道写入器
			ChannelReader：通道读取器
			

			ChannelServer：通道服务器
				为Channel对接到Pipeline提供服务
				

		管道：
			ChannelPipeline：处理管道
			ChannelFilter：过滤器	
			ChannelPipelineContext：管道上下文
			ChannelMessage：在管道流过的消息
	
