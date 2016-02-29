using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.Communication
{
    public abstract class Connection
    {
        //REPRESENTS A SINGLE PATH TO A SINGLE MACHINE
        public abstract void SendMessage(byte[] Message);
        public DateTime Expires;
    }
}
