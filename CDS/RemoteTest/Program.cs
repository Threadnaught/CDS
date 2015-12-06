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
            Remote.Start();
            Node n = Remote.FromName("127.0.0.1.TestNode");

            Console.WriteLine(n.GetIfExists());
            //for(int i = 0; i < 1000; i++) foreach (byte b in n.Read().Data) Console.Write(b.ToString() + " ");
        }
    }
}