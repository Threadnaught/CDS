using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.Communication
{
    public class Request
    {
        public Operation Op;
        public Guid SenderID;
        public Guid MessageID;
        public string TargetNode;
        public byte[] Body;
    }
}
