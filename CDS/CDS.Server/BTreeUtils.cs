using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;


//UNDER HEAVY CONSTRUCITON, CLEAN UP LATER

namespace CDS.Data
{
    public static class TableUtils 
    {
        static BPlusTree<Key, byte[]> tree;
        public static void Init() 
        {
            BPlusTree<Key, byte[]>.Options o = new BPlusTree<Key, byte[]>.Options(new KeySerializer(), PrimitiveSerializer.Bytes) 
            {
                CreateFile = CreatePolicy.IfNeeded,
                FileName = "Nodes.Dat" //change to .cds after finished            
            };
        }
    }

    enum TableType : byte
    {
        Nodes = 0, //storing information about nodes
        Children = 1, //storing information about whilch children nodes have
        Data = 2 //storing node contents
    }

    abstract class TableData 
    {
        public const int DATA_LEN = 512;
        abstract byte[] GetData();
        //static TableData ReconstructFromData(byte[] data);
    }
    





    struct Key 
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
