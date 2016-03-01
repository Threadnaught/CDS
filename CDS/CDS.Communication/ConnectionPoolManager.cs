using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CDS.Communication
{
    public static class MasterConnectionPool
    {
        //MANAGES ALL CONNECTIONS AND INTERFACES TO REST OF PROGRAM
        public static List<Connection> Connections = new List<Connection>();
        public static List<Client> Clients = new List<Client>();
        public static TimeSpan ExpiryTime = new TimeSpan(0, 10, 0);
        public static void MessageReceivedFromConnection(Stream s, ulong Length, Connection con)
        {
            //read message out of stream
            Operation MessageType;
            Guid SenderID;
            Guid MessageID;
            string TargetNode;
            byte[] Body;
            try
            {
                MessageType = (Operation)s.ReadByte();
                if ((byte)MessageType < 127)
                {
                    SenderID = new Guid(s.ReadFromStream(16));
                    MessageID = new Guid(s.ReadFromStream(16));
                    TargetNode = "";
                    int c;
                    while (true)
                    {
                        c = s.ReadByte();
                        if (c == 0) break;
                        TargetNode += (char)c;
                    }
                    Body = s.ReadFromStream((int)Length - (34 + TargetNode.Length));
                }
                else
                {

                }
            }
            catch
            {
                return;
            }
        }
        public static Client FindClient(Guid g)
        {
            foreach (Client c in Clients) if (c.ID == g) return c;
            return null;
        }
        public static byte[] ReadFromStream(this Stream s, int len)
        {
            byte[] ret = new byte[len];
            s.Read(ret, 0, len);
            return ret;
        }
    }
}
