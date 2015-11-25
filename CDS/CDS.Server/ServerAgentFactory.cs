using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CDS.Common;

namespace CDS.Server
{
    class ServerAgentFactory : AgentFactory
    {
        public override CDSAgent Open(int ChannelID, CDSMessageHandler Handler)
        {
            return null;
        }
    }
}