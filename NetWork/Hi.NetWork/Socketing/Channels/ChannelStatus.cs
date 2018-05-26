namespace Hi.NetWork.Socketing.Channels
{
    /// <summary>
    /// 通道状态
    /// </summary>
    public enum ChannelStatus : byte
    {
        Activity,           //已激活
        Connection,         //Connected表示是正常状态，也可以表示（已重连、已重启）
        Close,              //已关闭
        Breake,             //已断开
        Shutdown            //终止
    }
}