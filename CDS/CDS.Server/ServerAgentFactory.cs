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
        public override CDSAgent Open(bool LocalSideStart, int ChannelID, CDSMessageHandler Handler)
        {
            return null;
        }
    }
}
