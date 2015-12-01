using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;


//UNDER HEAVY CONSTRUCITON, CLEAN UP LATER

using CDS.Common;

namespace CDS.Data
{
    public static class TableUtils 
    {
        static BPlusTree<Key, byte[]> tree;
        public static void Init() 
        {
            BPlusTree<Key, byte[]>.Options o = new BPlusTree<Key, byte[]>.Options(new KeySerializer(), PrimitiveSerializer.Bytes, new KeyComparer()) 
            {
                CreateFile = CreatePolicy.IfNeeded,
                FileName = "Nodes.Dat" //change to Nodes.cds after finished
            };
            tree = new BPlusTree<Key, byte[]>(o);
        }
        static byte[] ReadFromTableRaw(Key k) 
        {
            return tree[k];
        }
        static void WriteToTableRaw(Key k, byte[] Data) 
        {
            tree[k] = Data;
        }
        public static TableData ReadFromTable(Key k) 
        {
            return TableData.ReconstructFromData(ReadFromTableRaw(k), k.Table);
        }
        public static void WriteToTable(Key k, TableData t) 
        {
            WriteToTableRaw(k, t.GetBytes());
        }
        public static void SetChildren(UInt32 Node, UInt32[] Children)
        {
            NodeData t = (NodeData)ReadFromTable(new Key() { Table = TableType.Nodes, Node = Node, Section = 0 });
            t.ChildLen = (uint)Children.Length;
            WriteToTable(new Key() { Table = TableType.Nodes, Node = Node, Section = 0 }, t);
            UInt32 RemainingChildren = t.ChildLen;
            UInt32 Offset = 0;
            int LoopCount = 0;
            while (RemainingChildren > (TableData.DATA_LEN / 4))
            {
                ChildData d = new ChildData() { Children = new UInt32[TableData.DATA_LEN / 4] };
                Array.Copy(Children, Offset, d.Children, 0, TableData.DATA_LEN / 4);
                WriteToTable(new Key() { Table = TableType.Children, Node = Node, Section = (uint)LoopCount }, d);
                RemainingChildren -= (TableData.DATA_LEN / 4);
                Offset += (TableData.DATA_LEN / 4);
                LoopCount++;
            }
            if (RemainingChildren > 0)
            {
                ChildData d = new ChildData() { Children = new UInt32[TableData.DATA_LEN / 4] };
                Array.Copy(Children, Offset, d.Children, 0, RemainingChildren);
                WriteToTable(new Key() { Table = TableType.Children, Node = Node, Section = (uint)LoopCount }, d);
            }
        }
        public static UInt32[] GetChildren(UInt32 Node) 
        {
            NodeData t = (NodeData)ReadFromTable(new Key() { Table = TableType.Nodes, Node = Node, Section = 0 });
            UInt32 RemainingChildren = t.ChildLen;
            UInt32 Offset = 0;
            int LoopCount = 0;
            UInt32[] ChildIDs = new UInt32[RemainingChildren];
            while (RemainingChildren > (TableData.DATA_LEN / 4)) 
            {
                //copying complete 128-child long blocks:
                ChildData c = (ChildData)ReadFromTable(new Key() { Table = TableType.Children, Node = Node, Section = (uint)LoopCount });
                c.Children.CopyTo(ChildIDs, Offset);
                RemainingChildren -= (TableData.DATA_LEN / 4);
                Offset += (TableData.DATA_LEN / 4);
                LoopCount++;
            }
            if (RemainingChildren > 0)
            {
                //copying the dregs:
                ChildData c = (ChildData)ReadFromTable(new Key() { Table = TableType.Children, Node = Node, Section = (uint)LoopCount });
                Array.Copy(c.Children, 0, ChildIDs, Offset, RemainingChildren);
            }
            return ChildIDs;
        }
        public static void SetBytes(UInt32 Node, byte[] Data) 
        {
            NodeData t = (NodeData)ReadFromTable(new Key() { Table = TableType.Nodes, Node = Node, Section = 0 });
            t.DataLen = (uint)Data.Length;
            WriteToTable(new Key() { Table = TableType.Nodes, Node = Node, Section = 0 }, t);
            UInt32 RemainingBytes = t.DataLen;
            UInt32 Offset = 0;
            int LoopCount = 0;
            while (RemainingBytes > TableData.DATA_LEN)
            {
                PayloadData d = new PayloadData() { Data = new byte[TableData.DATA_LEN] };
                Array.Copy(Data, Offset, d.Data, 0, TableData.DATA_LEN);
                WriteToTable(new Key() { Table = TableType.Data, Node = Node, Section = (uint)LoopCount }, d);
                RemainingBytes -= TableData.DATA_LEN;
                Offset += TableData.DATA_LEN;
                LoopCount++;
            }
            if (RemainingBytes > 0)
            {
                PayloadData d = new PayloadData() { Data = new byte[RemainingBytes] };
                Array.Copy(Data, Offset, d.Data, 0, RemainingBytes);
                WriteToTable(new Key() { Table = TableType.Data, Node = Node, Section = (uint)LoopCount }, d);
            }
        }
        public static byte[] GetBytes(UInt32 Node) 
        {
            UInt32 RemainingBytes = ((NodeData)ReadFromTable(new Key() { Table = TableType.Nodes, Node = Node, Section = 0 })).DataLen;
            UInt32 Offset = 0;
            int LoopCount = 0;
            byte[] Ret = new byte[RemainingBytes];
            while (RemainingBytes > TableData.DATA_LEN) 
            {
                PayloadData d = (PayloadData)ReadFromTable(new Key() { Table = TableType.Data, Node = 0, Section = (uint)LoopCount });
                d.Data.CopyTo(Ret, Offset);
                RemainingBytes -= TableData.DATA_LEN;
                Offset += TableData.DATA_LEN;
                LoopCount++;
            }
            if (RemainingBytes > 0)
            {
                PayloadData d = (PayloadData)ReadFromTable(new Key() { Table = TableType.Data, Node = 0, Section = (uint)LoopCount });
                d.Data.CopyTo(Ret, Offset);
            }
            return Ret;
        }
    }

    public enum TableType : byte
    {
        Nodes = 0, //storing information about nodes
        Children = 1, //storing information about whilch children nodes have
        Data = 2 //storing node contents
    }

    public abstract class TableData 
    {
        public const int DATA_LEN = 512;
        public abstract byte[] GetBytes();
        public static TableData ReconstructFromData(byte[] data, TableType type) 
        {
            switch (type) 
            {
                case TableType.Nodes:
                    return new NodeData(data);
                case TableType.Children:
                    return new ChildData(data);
                case TableType.Data:
                    return new PayloadData(data);
            }
            return null;
        }
    }
    public class NodeData : TableData
    {
        const int NAME_LEN = 32;
        public Int32 ParentID;
        public UInt32 ChildLen; //number of children node has
        public UInt32 DataLen; //length of data contained in node in bytes
        public string Name = "";
        public NodeType type;
        public override byte[] GetBytes()
        {
            byte[] Ret = new byte[DATA_LEN];
            BitConverter.GetBytes(ParentID).CopyTo(Ret, 0);
            BitConverter.GetBytes(ChildLen).CopyTo(Ret, 4);
            BitConverter.GetBytes(DataLen).CopyTo(Ret, 8);
            System.Text.Encoding.ASCII.GetBytes(Name).CopyTo(Ret, 12);
            Ret[12 + NAME_LEN] = (byte)type;
            return Ret;
        }
        public NodeData() { }
        public NodeData(byte[] Data) 
        {
            ParentID = BitConverter.ToInt32(Data, 0);
            ChildLen = BitConverter.ToUInt32(Data, 4);
            DataLen = BitConverter.ToUInt32(Data, 8);
            Name = System.Text.Encoding.ASCII.GetString(Data, 12, NAME_LEN);
            Name = Name.Trim(new char[] { (char)0 }); // trim out trailing 0es
            type = (NodeType)Data[12 + NAME_LEN];
        }
    }
    public class ChildData : TableData 
    {
        public UInt32[] Children;
        public override byte[] GetBytes()
        {
            byte[] ret = new byte[Children.Length * 4];
            for (int i = 0; i < Children.Length; i++) 
            {
                BitConverter.GetBytes(Children[i]).CopyTo(ret, i * 4);
            }
            return ret;
        }
        public ChildData() { }
        public ChildData(byte[] Data) 
        {
            int Len = DATA_LEN / 4;
            Children = new UInt32[Len];
            for(int i = 0; i < Len; i++)
            {
                Children[i] = BitConverter.ToUInt32(Data, i * 4);
            }
        }
    }
    public class PayloadData : TableData 
    {
        public byte[] Data;
        public override byte[] GetBytes()
        {
            return Data;
        }
        public PayloadData() { }
        public PayloadData(byte[] data) 
        {
            Data = data;
        }
    }




    public struct Key 
    {
        public TableType Table;
        public UInt32 Node;
        public UInt32 Section;
    }
    class KeySerializer : ISerializer<Key> 
    {
        public void WriteTo(Key b, Stream s)
        {
            s.WriteByte((byte)b.Table);
            s.Write(BitConverter.GetBytes(b.Node), 0, 4);
            s.Write(BitConverter.GetBytes(b.Section), 0, 4);
        }

        public Key ReadFrom(Stream s)
        {
            Key ret = new Key();
            ret.Table = (TableType)s.ReadByte();
            ret.Node = BitConverter.ToUInt32(ReadNextBytes(4, s), 0);
            ret.Section = BitConverter.ToUInt32(ReadNextBytes(4, s), 0);
            return ret;
        }
        static byte[] ReadNextBytes(int length, Stream s)
        {
            byte[] bs = new byte[length];
            s.Read(bs, 0, length);
            return bs;
        }
    }
    class KeyComparer : IComparer<Key> 
    {
        public int Compare(Key x, Key y)
        {
            if (x.Table < y.Table) return -1;
            if (x.Table > y.Table) return 1;

            if (x.Node < y.Node) return -1;
            if (x.Node > y.Node) return 1;

            if (x.Section < y.Section) return -1;
            if (x.Section > y.Section) return 1;

            return 0;

        }
    }
}
