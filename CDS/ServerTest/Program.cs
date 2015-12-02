﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CDS.Common;
using CDS.Data;

class Program
{
    static void Main(string[] args)
    {
        File.Delete("Nodes.cds");
        TableUtils.Init();
        TcpConnectionListener.AgentCreators.Add(false, new ServerAgentFactory());
        TcpConnectionListener.Init();
    }
}