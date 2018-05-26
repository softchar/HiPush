


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
			Channel：IO读写通道
			ChannelInput：通道输入流
			ChannelWriter：通道写入器
			ChannelReader：通道读取器
			ChannelOutput：通道输出流

			ChannelServer：通道服务器
				为Channel对接到Pipeline提供服务
				

		管道：
			ChannelPipeline：处理管道
			ChannelFilter：过滤器	
			ChannelPipelineContext：管道上下文
			ChannelMessage：在管道流过的消息
	
