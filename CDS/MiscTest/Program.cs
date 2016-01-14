using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CDS.Common;
using CDS.Data;

namespace MiscTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MemoryStream RequestStream = new MemoryStream(new byte[] { 
                /*len*/0, 0, 0, 0, 0, 0, 0, 0, 
                /*Message type*/(byte)MessageType.create, 
                /*SenderId*/1, 2, 3, 4, 5, 6, 7, 8,
                /*MessageID*/9, 10, 11, 12, 13, 14, 15, 16,
                /*null terminated target node*/ (byte)'t', (byte)'e',(byte)'s', (byte)'t', 0,
                /*data*/(byte)NodeType.Hollow,(byte)NodeType.Hollow,(byte)NodeType.Hollow,
            });
            RequestStream.Seek(9, SeekOrigin.Begin);
            Request r = Request.ParseFromStream(RequestStream, (ulong)RequestStream.Length, MessageType.create);
            Console.ReadKey();
        }
    }
}