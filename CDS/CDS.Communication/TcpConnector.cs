using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace CDS.Communication
{
    class TcpConnector : Connector
    {
        public override bool CanCreate(string ConnectionCode)
        {
            return ConnectionCode.Split(':')[0] == "T";
        }

        public override Connection Create(string ConnectionCode)
        {
            return new TcpConnection(IPAddress.Parse(ConnectionCode.Split(':')[0]));
        }
    }
}