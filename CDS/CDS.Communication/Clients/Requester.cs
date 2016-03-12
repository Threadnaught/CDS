using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.Communication
{
    public class Requester
    {
        //makes requests to remote server, and processes responses from server
        public Guid RequesterID; //this ID
        public List<Request> OutstandingRequests = new List<Request>();
        public void ReceiveResponse(Response r)
        {
            foreach (Request re in OutstandingRequests)
            {
                if (re.MessageID == r.MessageID)
                {
                    
                }
            }
        }
        public bool HasRequest(Guid MessageID)
        {
            return OutstandingRequests.FirstOrDefault(r => { return r.MessageID == MessageID; }) != null;
        }
    }
}
