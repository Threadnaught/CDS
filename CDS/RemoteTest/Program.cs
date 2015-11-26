using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CDS.Common;
using CDS.Remote;

namespace RemoteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            CDSMessageHandler h = new CDSMessageHandler(System.Net.IPAddress.Parse("127.0.0.1"));
            h.agentFactories = new Dictionary<bool, AgentFactory>();
            h.agentFactories.Add(true, new RemoteAgentFactory());
            CDSRemoteAgent a = (CDSRemoteAgent)h.OpenNewChannel();
            RemoteNode n = a.Root.AddChild("Test", NodeType.Data);
            while (true)
            {
                n.Write(new byte[10000000]);
                Console.WriteLine(n.Read().Length);
            }
        }
    }
}