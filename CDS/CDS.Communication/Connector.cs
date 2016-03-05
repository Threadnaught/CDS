using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.Communication
{
    public abstract class Connector
    {
        //manages connecting to and connections from other machines
        public abstract bool CanCreate(string ConnectionCode);
        public abstract Connection Create(string ConnectionCode);
        
    }
}
