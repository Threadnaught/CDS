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
            Node n = LocalNode.Root;
            Node child = n.AddChild(NodeType.Data, "Test");
            child.Delete();
            Console.WriteLine(n.Children().Count);

            Console.WriteLine();
            Console.ReadKey();
        }
    }
}