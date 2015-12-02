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
            System.IO.File.Delete("Nodes.dat");
            TableUtils.Init();
            Node n = LocalNode.Root;
            Node child = n.AddChild(NodeType.Data, "Test");
            child.Delete();
            Console.WriteLine(n.Children().Count);
            child = n.AddChild(NodeType.Data, "Test");
            child.Write(new CDSData() { Type = DataType.Data, Data = new byte[1000] });
            Console.WriteLine(child.Read());
            Console.WriteLine(n.Children().Count);
            for (int i = 0; i < 200; i++) 
            {
                n.AddChild(NodeType.Hollow, i.ToString());
            }
            Console.WriteLine(n.Children().Count);
            Console.ReadKey();
        }
    }
}