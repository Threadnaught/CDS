using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CDS.Communication
{
    public static class Utils
    {

        public static byte[] ReadFromStream(this Stream s, int len)
        {
            //util to read len bytes from stream s
            byte[] ret = new byte[len];
            s.Read(ret, 0, len);
            return ret;
        }
    }
}
