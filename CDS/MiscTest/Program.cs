using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CDS.Common;

namespace MiscTest
{
    class Program
    {
        static void Main(string[] args)
        {
            CDSData d = CDSData.FromRaw(new byte[]{0, 1, 1, 1});
            Console.WriteLine(d.Type);
            Console.WriteLine(d.Data.Length);
            foreach (byte b in d.ToRaw()) Console.Write(b.ToString() + " ");
            Console.ReadKey();
        }
    }
}
