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
            System.Threading.Thread.Sleep(100);
            CDSMessageHandler h = new CDSMessageHandler(System.Net.IPAddress.Parse("127.0.0.1"));
            h.agentFactories = new Dictionary<bool, AgentFactory>();
            h.agentFactories.Add(true, new RemoteAgentFactory());
            CDSRemoteAgent a = (CDSRemoteAgent)h.OpenNewChannel();
            List<Node> children = a.Root.GetChildren();
            Node n = children.Find(m => m.GetName() == "testnode");
            n.Write(new CDSData() { Data = new byte[]{ 1, 2, 3}, Type = DataType.Data });
        }
    }
}