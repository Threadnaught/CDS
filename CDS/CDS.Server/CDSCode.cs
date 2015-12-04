using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;

using CDS.Common;

namespace CDS.Data
{
    public class CDSCode
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
            CDSData data = new CDSData();
            Engine e = new Engine();
            e.Execute(Code);
            e.SetValue("OutData", data);
            e.Execute("OnRead();");
            return data;
        }
        public void Write(CDSData data)
        {
            Engine e = new Engine();
            e.Execute(Code);
            e.SetValue("InData", data);
            e.Execute("OnWrite();");
        }
    }
}
