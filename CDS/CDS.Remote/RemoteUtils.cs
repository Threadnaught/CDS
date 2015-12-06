using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

using CDS.Common;

namespace CDS.Remote
{
    public static class Remote
    {
        public static void Start()
        {
            CDSMessageHandler.agentFactories.Add(true, new RemoteAgentFactory());
        }
        public static RemoteNode FromName(string Name) 
        {
            //add link capability
            string[] Sections = Name.Split('.');
            int Len = Sections[0].Length + Sections[1].Length + Sections[2].Length + Sections[3].Length + 3;
            IPAddress Address = IPAddress.Parse(Name.Substring(0, Len));

            CDSMessageHandler h = new CDSMessageHandler(Address);
            RemoteNode n = ((CDSRemoteAgent)h.OpenNewChannel()).Root;

            for (int i = 4; i < Sections.Length; i++) 
            {
                n = (RemoteNode)n.ChildByName(Sections[i]);
            }
            return n;
        }
    }
}