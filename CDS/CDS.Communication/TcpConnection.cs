using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace CDS.Communication
{
    public class TcpConnection : Connection
    {
        TcpClient c = new TcpClient();
        public override void SendMessage(byte[] Message)
        {

        }

        protected override Stream GetStream()
        {
            return c.GetStream();
        }

        protected override bool MessageIncoming()
        {
            return c.GetStream().DataAvailable;
        }

        protected override ulong MessageLength()
        {
            byte[] Length = new byte[8];
            return BitConverter.ToUInt64(Length, 0);
        }
    }
}
