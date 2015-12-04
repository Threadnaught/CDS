using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CDS.Common;
using CDS.Data;

namespace MiscTest
{
    class Program
    {
        static void Main(string[] args)
        {
            CDSCode c = new CDSCode() { Code = @"function OnRead()
{
    var v = new CDSCommon.CDSData();
    v.Data = [1, 2, 3, 4, 5];
    return v;
}
function OnWrite(data)
{
    Log(data.Data[0]);
}" };
            foreach (byte b in c.Read().Data) 
            {
                Console.Write(b.ToString() + " ");
            }
            c.Write(new CDSData() { Data = new byte[]{5, 4, 3, 2, 1}, Type = DataType.Data });
            Console.ReadKey();
        }
    }
}