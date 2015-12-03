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
            CDSCode c = new CDSCode() { Code = @"function OnRead(){OutData.Data = [1, 2, 3, 4, 5]}" };
            foreach (byte b in c.Read().Data) 
            {
                Console.Write(b.ToString() + " ");
            }
            Console.ReadKey();
        }
    }
}