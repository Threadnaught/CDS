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
        public override CDSAgent Open(int ChannelID, CDSMessageHandler Handler)
        {
            return new CDSLocalAgent() { CDSHandler = Handler, ChannelID = ChannelID };
        }
    }
}