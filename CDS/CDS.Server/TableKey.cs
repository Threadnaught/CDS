using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;

using CDS.Common;

namespace CDS.Data
{
    public struct TableKey
    {
        public TableType Table;
        public UInt32 Node;
        public UInt32 Section;
    }
    class KeySerializer : ISerializer<TableKey>
    {
        public void WriteTo(TableKey b, Stream s)
        {
            s.WriteByte((byte)b.Table);
            s.Write(BitConverter.GetBytes(b.Node), 0, 4);
            s.Write(BitConverter.GetBytes(b.Section), 0, 4);
        }

        public TableKey ReadFrom(Stream s)
        {
            TableKey ret = new TableKey();
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
    class KeyComparer : IComparer<TableKey>
    {
        public int Compare(TableKey x, TableKey y)
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
