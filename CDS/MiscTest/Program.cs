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
            System.IO.File.Delete("Nodes.dat"); //change when finished
            TableUtils.Init();
            TableUtils.WriteToTable(new Key() { Table = TableType.Nodes, Node = 0, Section = 0 }, new NodeData() { ChildLen = (TableData.DATA_LEN / 4 ) * 2 });
            byte[] arrrrr = new byte[400];
            for (int i = 0; i < arrrrr.Length; i++) 
            {
                arrrrr[i] = (byte)(i % 100);
            }
            TableUtils.SetBytes(0, arrrrr);
            foreach (byte b in TableUtils.GetBytes(0)) 
            {
                Console.WriteLine(b);
            }
            Console.ReadKey();
        }
    }
}