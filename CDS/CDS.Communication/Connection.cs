using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CDS.Communication
{
    public abstract class Connection
    {
        //REPRESENTS A SINGLE PATH TO A SINGLE MACHINE
        public abstract void SendMessage(byte[] Message);
        public DateTime Expires;
        public bool Expired;
        
        public void Check()
        {
            if (MessageIncoming())
            {
                MasterConnectionPool.MessageReceivedFromConnection(GetStream(), MessageLength(), this);
                Expires = DateTime.Now + MasterConnectionPool.ExpiryTime;
            }
            if (Expires > DateTime.Now || Closed()) { Expired = true; return; }
            Task.Factory.StartNew(Check);
        }

        protected abstract Stream GetStream();

        protected abstract bool Closed();
        protected abstract bool MessageIncoming();
        protected abstract ulong MessageLength();
    }
}
