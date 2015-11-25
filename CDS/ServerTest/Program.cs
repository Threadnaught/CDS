using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CDS.Common;
using CDS.Server;

class Program
{
    static void Main(string[] args)
    {
        File.Delete("Nodes.cds");
        SqliteWrapper.Init();
        TcpConnectionListener.AgentCreators.Add(false, new ServerAgentFactory());
        TcpConnectionListener.Init();
    }
}