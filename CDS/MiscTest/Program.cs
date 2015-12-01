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
            TableUtils.Init();
            TableUtils.WriteToTable(new Key() { Table = TableType.Nodes, Node = 0, Section = 0 }, new NodeData() { Name = "root", type = NodeType.Hollow, ParentID = -1 });
            NodeData d = (NodeData)TableUtils.ReadFromTable(new Key() { Table = TableType.Nodes, Node = 0, Section = 0 });
            Console.WriteLine(d.Name.Length);
            Console.ReadKey();
        }
    }
}
