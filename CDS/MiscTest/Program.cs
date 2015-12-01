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
            TableUtils.SetChildren(0, new UInt32[400]);
            Console.WriteLine(TableUtils.GetChildren(0).Length);
            Console.ReadKey();
        }
    }
}