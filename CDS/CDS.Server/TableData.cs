using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CDS.Common;

namespace CDS.Data
{
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
                case TableType.Meta:
                    return new MetaData(data);
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
            for (int i = 0; i < Len; i++)
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
    public class MetaData : TableData 
    {
        public UInt32 NextAvailableId = 0;
        public override byte[] GetBytes()
        {
            return BitConverter.GetBytes(NextAvailableId);
        }
        public MetaData() { }
        public MetaData(byte[] data) 
        {
            NextAvailableId = BitConverter.ToUInt32(data, 0);
        }
    }
}
