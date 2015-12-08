using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CDS.Common;

namespace CDS.Remote
{
    public static class RemoteAgentFactory
    {
        public static CDSRemoteAgent OpenChannel(this CDSMessageHandler h)
        {
            int ID = 0;
            foreach (int NewID in h.Agents.Keys)
            {
                if (ID <= NewID)
                    ID = NewID + 1;
            }
            h.chan.CreateChannel(ID);
            h.Agents.Add(ID, new CDSRemoteAgent() { CDSHandler = h, ChannelID = ID });
            return (CDSRemoteAgent)h.Agents[ID];
        }
    }
}