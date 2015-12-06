using System.Collections.Generic;
namespace CDS.Common 
{
    public abstract class Agent
    {
        public static Dictionary<int, Agent> Agents = new Dictionary<int, Agent>();
        public CDSMessageHandler CDSHandler;
        public abstract void OnReceiveCDSMessage(byte Op, string TgtNode, int OpID, byte[] Body);
    }

    public abstract class AgentFactory
    {
        public abstract Agent Open(int ChannelID, CDSMessageHandler Handler);
    }
}