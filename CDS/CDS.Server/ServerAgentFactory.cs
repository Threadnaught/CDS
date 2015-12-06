using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CDS.Common;

namespace CDS.Data
{
    public class ServerAgentFactory : AgentFactory
    {
        public override Agent Open(int ChannelID, CDSMessageHandler Handler)
        {
            return new LocalAgent() { CDSHandler = Handler, ChannelID = ChannelID };
        }
    }
}