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
            try
            {
                Operation MessageType;
                Guid SenderID;
                Guid MessageID;
                string TargetNode;
                byte[] Body;
                MessageType = (Operation)s.ReadByte();
                if ((byte)MessageType < 127)
                {
                    //request type (sender id, message id, target, body)
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
                    //response type (message id, body)
                    MessageID = new Guid(s.ReadFromStream(16));
                    Client c = FindMessageClient(MessageID);
                    SenderID = c.ID;
                    //Message m = c.OutgoingMessages.First(me => { return me.MessageID == MessageID; } );
                    TargetNode = m.TargetNode;
                    Body = s.ReadFromStream((int)Length - 17);
                }
                //Message Constructed = new Message() { MessageID = MessageID, Op = MessageType, SenderID = SenderID, TargetNode = TargetNode };
                //FindClient(Constructed.SenderID).ReceivedMessageFromConnection(con, Constructed);
            }
            catch
            {
                con.Closed();
            }
        }
        public static Client FindClient(Guid g)
        {
            //find client by guid
            foreach (Client c in Clients) if (c.ID == g) return c;
            return null;
        }
        public static Client FindMessageClient(Guid message)
        {
            //find client by guid of message
            //foreach (Client c in Clients)
                //foreach (Message m in c.OutgoingMessages)
                  //  if (m.MessageID == message) return c;
            return null;
        }
        public static byte[] ReadFromStream(this Stream s, int len)
        {
            //util to read len bytes from stream s
            byte[] ret = new byte[len];
            s.Read(ret, 0, len);
            return ret;
        }
    }
}
