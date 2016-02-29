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
        public static void MessageReceivedFromConnection(Stream s, int Length, Connection con)
        {

        }
    }
}
