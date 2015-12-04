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
            return new CDSCode() { Code = Encoding.Unicode.GetString(Raw) };
        }
        public byte[] ToRaw() 
        {
            return Encoding.Unicode.GetBytes(Code);
        }
        public CDSData Read()
        {
            Engine e = PrepareEngine(Code);
            return (CDSData)e.Invoke("OnRead", new Object[0]).ToObject();
        }
        public void Write(CDSData data)
        {
            Engine e = PrepareEngine(Code);
            e.Invoke("OnWrite", new Object[]{ data });
        }
        static Engine PrepareEngine(string code) 
        {
            Engine e = new Engine(f => f.AllowClr(typeof(CDSData).Assembly));
            e.SetValue("Log", new Action<Object>(Console.WriteLine)); //change to write data to a node rather than to console later
            e.Execute("var CDSCommon = importNamespace('CDS.Common');");
            e.Execute(code);
            return e;
        }
    }
    public enum CodeType : byte
    {
        JS = 0,
        CSharp = 1 //when roslyn is added
    }
}