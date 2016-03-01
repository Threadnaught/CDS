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

        }
        public static Client FindClient(Guid g)
        {
            foreach (Client c in Clients) if (c.ID == g) return c;
            return null;
        }
    }
}
