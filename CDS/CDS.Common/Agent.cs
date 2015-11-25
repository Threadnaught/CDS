namespace CDS.Common 
{
    public abstract class CDSAgent
    {
        public CDSMessageHandler CDSHandler;
        public abstract void OnReceiveCDSMessage(byte Op, string TgtNode, int OpID, byte[] Body);
    }

    public abstract class AgentFactory
    {
        public abstract CDSAgent Open(int ChannelID, CDSMessageHandler Handler);
    }
}