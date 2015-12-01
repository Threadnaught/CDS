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
            }
            return null;
        }
    }
    public class NodeData : TableData
    {
        public const int NAME_LEN = 32;
        public Int32 ParentID;
        public UInt32 ChildLen; //number of children node has
        public UInt32 DataLen; //length of data contained in node in bytes
        public string Name;
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
