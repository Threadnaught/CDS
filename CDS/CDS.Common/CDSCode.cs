using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.Common
{
    class CDSCode
    {
        public string Code;
        public CDSCode() { }
        public static CDSCode FromRaw(byte[] Raw) 
        {
            return new CDSCode() { Code = Encoding.Unicode.GetString(Raw); };
        }
        public byte[] ToRaw() 
        {
            return Encoding.Unicode.GetBytes(Code);
        }
        public CDSData Read() 
        {
            CDSData data;
            
        }
    }
}