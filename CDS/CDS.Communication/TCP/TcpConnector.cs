using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CDS.Communication
{
    class TcpConnector : Connector
    {
        TcpListener l;
        public TcpConnector()
        {
            l = new TcpListener(IPAddress.Parse("127.0.0.1"), TcpConnection.Port);
            l.Start();
        }
        public override bool CanCreate(string ConnectionCode)
        {
            return ConnectionCode.Split(':')[0] == "T";
        }
        public override Connection Create(string ConnectionCode)
        {
            return new TcpConnection(IPAddress.Parse(ConnectionCode.Split(':')[0]));
        }
        public override bool ConnectionWaiting()
        {
            return l.Pending();
        }
        public override Connection ReceiveConnection()
        {
            return new TcpConnection(l.AcceptTcpClient());
        }
    }
}