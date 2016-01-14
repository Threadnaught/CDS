using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;

namespace CDS.Common
{
    public static class TableUtils
    {
        static BPlusTree<TableKey, byte[]> table;
        public static void Init()
        {
            if (!File.Exists("Nodes.dat"))
            {
                BPlusTree<TableKey, byte[]>.Options o = new BPlusTree<TableKey, byte[]>.Options(new KeySerializer(), PrimitiveSerializer.Bytes, new KeyComparer())
                {
                    CreateFile = CreatePolicy.IfNeeded,
                    FileName = "Nodes.Dat"
                };
                table = new BPlusTree<TableKey, byte[]>(o);
                WriteToTable(new TableKey() { Table = TableType.Meta, Node = 0, Section = 0 }, new MetaData() { NextAvailableId = 0 });
                WriteToTable(new TableKey() { Table = TableType.Nodes, Node = 0, Section = 0 }, new NodeData() { Name = "root", ParentID = -1, type = NodeType.Hollow });
            }
            else
            {
                BPlusTree<TableKey, byte[]>.Options o = new BPlusTree<TableKey, byte[]>.Options(new KeySerializer(), PrimitiveSerializer.Bytes, new KeyComparer())
                {
                    CreateFile = CreatePolicy.IfNeeded,
                    FileName = "Nodes.Dat"
                };
                table = new BPlusTree<TableKey, byte[]>(o);
            }
        }
        static byte[] ReadFromTableRaw(TableKey k)
        {
            return table[k];
        }
        static void WriteToTableRaw(TableKey k, byte[] Data)
        {
            table[k] = Data;
        }
        public static TableData ReadFromTable(TableKey k)
        {
            return TableData.ReconstructFromData(ReadFromTableRaw(k), k.Table);
        }
        public static void WriteToTable(TableKey k, TableData t)
        {
            WriteToTableRaw(k, t.GetBytes());
        }
        public static void SetNodeData(UInt32 Node, NodeData data)
        {
            WriteToTable(new TableKey() { Table = TableType.Nodes, Node = Node, Section = 0 }, data);
        }
        public static NodeData GetNodeData(UInt32 Node)
        {
            return (NodeData)ReadFromTable(new TableKey() { Table = TableType.Nodes, Node = Node, Section = 0 });
        }
        public static void SetChildren(UInt32 Node, UInt32[] Children)
        {
            NodeData t = (NodeData)ReadFromTable(new TableKey() { Table = TableType.Nodes, Node = Node, Section = 0 });
            t.ChildLen = (uint)Children.Length;
            WriteToTable(new TableKey() { Table = TableType.Nodes, Node = Node, Section = 0 }, t);
            UInt32 RemainingChildren = t.ChildLen;
            UInt32 Offset = 0;
            int LoopCount = 0;
            while (RemainingChildren > (TableData.DATA_LEN / 4))
            {
                ChildData d = new ChildData() { Children = new UInt32[TableData.DATA_LEN / 4] };
                Array.Copy(Children, Offset, d.Children, 0, TableData.DATA_LEN / 4);
                WriteToTable(new TableKey() { Table = TableType.Children, Node = Node, Section = (uint)LoopCount }, d);
                RemainingChildren -= (TableData.DATA_LEN / 4);
                Offset += (TableData.DATA_LEN / 4);
                LoopCount++;
            }
            if (RemainingChildren > 0)
            {
                ChildData d = new ChildData() { Children = new UInt32[TableData.DATA_LEN / 4] };
                Array.Copy(Children, Offset, d.Children, 0, RemainingChildren);
                WriteToTable(new TableKey() { Table = TableType.Children, Node = Node, Section = (uint)LoopCount }, d);
            }
        }
        public static UInt32[] GetChildren(UInt32 Node)
        {
            NodeData t = (NodeData)ReadFromTable(new TableKey() { Table = TableType.Nodes, Node = Node, Section = 0 });
            UInt32 RemainingChildren = t.ChildLen;
            UInt32 Offset = 0;
            int LoopCount = 0;
            UInt32[] ChildIDs = new UInt32[RemainingChildren];
            while (RemainingChildren > (TableData.DATA_LEN / 4))
            {
                //copying complete 128-child long blocks:
                ChildData c = (ChildData)ReadFromTable(new TableKey() { Table = TableType.Children, Node = Node, Section = (uint)LoopCount });
                c.Children.CopyTo(ChildIDs, Offset);
                RemainingChildren -= (TableData.DATA_LEN / 4);
                Offset += (TableData.DATA_LEN / 4);
                LoopCount++;
            }
            if (RemainingChildren > 0)
            {
                //copying the dregs:
                ChildData c = (ChildData)ReadFromTable(new TableKey() { Table = TableType.Children, Node = Node, Section = (uint)LoopCount });
                Array.Copy(c.Children, 0, ChildIDs, Offset, RemainingChildren);
            }
            return ChildIDs;
        }
        public static void SetBytes(UInt32 Node, byte[] Data)
        {
            NodeData t = (NodeData)ReadFromTable(new TableKey() { Table = TableType.Nodes, Node = Node, Section = 0 });
            t.DataLen = (uint)Data.Length;
            WriteToTable(new TableKey() { Table = TableType.Nodes, Node = Node, Section = 0 }, t);
            UInt32 RemainingBytes = t.DataLen;
            UInt32 Offset = 0;
            int LoopCount = 0;
            while (RemainingBytes > TableData.DATA_LEN)
            {
                PayloadData d = new PayloadData() { Data = new byte[TableData.DATA_LEN] };
                Array.Copy(Data, Offset, d.Data, 0, TableData.DATA_LEN);
                WriteToTable(new TableKey() { Table = TableType.Data, Node = Node, Section = (uint)LoopCount }, d);
                RemainingBytes -= TableData.DATA_LEN;
                Offset += TableData.DATA_LEN;
                LoopCount++;
            }
            if (RemainingBytes > 0)
            {
                PayloadData d = new PayloadData() { Data = new byte[RemainingBytes] };
                Array.Copy(Data, Offset, d.Data, 0, RemainingBytes);
                WriteToTable(new TableKey() { Table = TableType.Data, Node = Node, Section = (uint)LoopCount }, d);
            }
        }
        public static byte[] GetBytes(UInt32 Node)
        {
            UInt32 RemainingBytes = ((NodeData)ReadFromTable(new TableKey() { Table = TableType.Nodes, Node = Node, Section = 0 })).DataLen;
            UInt32 Offset = 0;
            int LoopCount = 0;
            byte[] Ret = new byte[RemainingBytes];
            while (RemainingBytes > TableData.DATA_LEN)
            {
                PayloadData d = (PayloadData)ReadFromTable(new TableKey() { Table = TableType.Data, Node = Node, Section = (uint)LoopCount });
                d.Data.CopyTo(Ret, Offset);
                RemainingBytes -= TableData.DATA_LEN;
                Offset += TableData.DATA_LEN;
                LoopCount++;
            }
            if (RemainingBytes > 0)
            {
                PayloadData d = (PayloadData)ReadFromTable(new TableKey() { Table = TableType.Data, Node = Node, Section = (uint)LoopCount });
                d.Data.CopyTo(Ret, Offset);
            }
            return Ret;
        }
        public static UInt32 GetLowestAvailableID()
        {
            MetaData d = (MetaData)ReadFromTable(new TableKey() { Table = TableType.Meta, Node = 0, Section = 0 });
            UInt32 ret = d.NextAvailableId;
            d.NextAvailableId++;
            WriteToTable(new TableKey() { Table = TableType.Meta, Node = 0, Section = 0}, d);
            return ret;
        }
        public static void Remove(UInt32 Node)
        {
            table.Remove(new TableKey() { Table = TableType.Nodes, Node = Node, Section = 0 });
        }
    }
    public enum TableType : byte
    {
        Nodes = 0, //storing information about nodes
        Children = 1, //storing information about whilch children nodes have
        Data = 2, //storing node contents
        Meta = 4 //storing metadata about table
    }
}