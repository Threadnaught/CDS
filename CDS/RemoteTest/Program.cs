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
            Node n = a.Root.ChildByName("TestNode");

            Console.WriteLine(n.GetIfExists());
            //for(int i = 0; i < 1000; i++) foreach (byte b in n.Read().Data) Console.Write(b.ToString() + " ");

            h.chan.MessageProvider.Alive = false;
        }
    }
}