using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CDS.Common;

namespace CDS.Remote
{
    public class RemoteAgentFactory : AgentFactory
    {
        public override CDSAgent Open(int ChannelID, CDSMessageHandler Handler)
        {
            return new CDSRemoteAgent() { CDSHandler = Handler, ChannelID = ChannelID };
        }
    }
}
