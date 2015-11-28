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
            RemoteNode n = (RemoteNode)a.Root.AddChild(NodeType.Data, "Test");
            while (true)
            {
                n.Write(new CDSData(DataType.Data, new byte[10000]));
                Console.WriteLine(n.Read().Data.Length);
            }
        }
    }
}