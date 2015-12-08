using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CDS.Common;

namespace CDS.Data
{
    public static class Server
    {
        public static void Start() 
        {
            TableUtils.Init();
            CDSMessageHandler.RemoteOpenFactory = new ServerAgentFactory();
            TcpConnectionListener.Init();
        }
    }
}