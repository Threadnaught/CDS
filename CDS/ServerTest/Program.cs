using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CDS.Common;
using CDS.Data;

class Program
{
    static void Main(string[] args)
    {
        File.Delete("Nodes.dat");
        Server.Start();
        LocalNode n = (LocalNode)LocalNode.Root.AddChild(NodeType.Stream, "TestNode");
        n.WriteRaw(new CDSCode() { Code = @"
function OnWrite(data){Log(data.Data.length)}
function OnRead(){
    var ret = new CDSCommon.CDSData(); 
    ret.Data = [new Date().getHours(), new Date().getMinutes(), new Date().getSeconds(), 0, 0];
    return ret; 
}"
        }.ToRaw());
    }
}