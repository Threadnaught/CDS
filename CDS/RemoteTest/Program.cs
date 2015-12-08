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
            System.Threading.Thread.Sleep(1000);

            CDSMessageHandler h = new CDSMessageHandler(System.Net.IPAddress.Parse("127.0.0.1"));
            CDSRemoteAgent[] agents = new CDSRemoteAgent[500];
            for (int i = 0; i < 500; i++)
            {
                agents[i] = (CDSRemoteAgent)h.OpenChannel();
            }
            while (true)
            {
                foreach (CDSRemoteAgent a in agents)
                {
                    Console.WriteLine(a.Root.ChildByName("TestNode").Read().Data.First());
                }
            }
            Node n = Remote.FromName("127.0.0.1.TestNode");

            //Console.WriteLine(n.GetIfExists());
            while(true) foreach (byte b in n.Read().Data) Console.Write(b.ToString() + "\t");
        }
    }
}