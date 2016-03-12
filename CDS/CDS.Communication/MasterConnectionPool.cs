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
        public static List<Requester> Requesters = new List<Requester>();
        public static List<Responder> Responders = new List<Responder>();
        public static TimeSpan ExpiryTime = new TimeSpan(0, 10, 0);
        public static void MessageReceivedFromConnection(Stream s, ulong Length, Connection con)
        {
            int op = s.ReadByte();
            if (op < 127)
            {
                //request, direct to responder
                RequestReceivedFromConnection(s, Length - 1, con, (Operation)op);
            }
            else
            {
                //response, direct to requester
                ResponseReceivedFromConnection(s, Length - 1, con, (Operation)op);
            }
        }
        public static void RequestReceivedFromConnection(Stream s, ulong Length, Connection con, Operation op)
        {
            Request r = new Request();
            //build request from stream
            r.Op = op;
            r.SenderID = new Guid(s.ReadFromStream(16));
            r.MessageID = new Guid(s.ReadFromStream(16));
            r.TargetNode = "";
            while (true)
            {
                int c = s.ReadByte();
                if (c == 0) break;
                r.TargetNode += (char)c;
            }
            r.Body = s.ReadFromStream((int)Length - (r.TargetNode.Length + 32));
            foreach (Responder re in Responders) if (re.RequesterID == r.SenderID)
                {
                    re.ReceiveRequest(r, con);
                    return;
                }
        }
        public static void ResponseReceivedFromConnection(Stream s, ulong Length, Connection con, Operation op)
        {
            Response r = new Response();
            r.MessageID = new Guid(s.ReadFromStream(16));
            r.Body = s.ReadFromStream((int)Length - 16);
            foreach (Requester re in Requesters) if (re.HasRequest(r.MessageID)) re.ReceiveResponse(r);
        }
        public static void NewConnection(Connection c)
        {
            Connections.Add(c);
        }
    }
}
