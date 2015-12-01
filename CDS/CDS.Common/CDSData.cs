using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.Common
{
    public class CDSData
    {
        public CDSData() { }
        public CDSData(DataType type, byte[] data) 
        {
            Type = type;
            Data = data;
        }
        public DataType Type;
        public byte[] Data;
        public static CDSData FromRaw(byte[] Raw) 
        {
            CDSData ret = new CDSData() { Type = (DataType)Raw[0] };
            ret.Data = new byte[Raw.Length - 1];
            Array.Copy(Raw, 1, ret.Data, 0, ret.Data.Length);
            return ret;
        }
        public byte[] ToRaw() 
        {
            byte[] ret = new byte[Data.Length + 1];
            ret[0] = (byte)Type;
            Array.Copy(Data, 0, ret, 1, Data.Length);
            return ret;
        }
    }
}
