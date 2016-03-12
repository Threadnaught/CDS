using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.Communication
{
    public class Responder
    {
        //takes requests from remote client, and sends responds to client
        public Guid RequesterID; //ID of remote requester
        public void ReceiveRequest(Request r, Connection c)
        {

        }
    }
}