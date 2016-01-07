using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Security.Cryptography;

namespace CDS.Common
{
    //desperatley needs auditing by somone who knows their shit
    static class NonCollingRNG
    {
        static Random rand;
        static byte[] ToAdd;
        static SHA512 hasher = new SHA512Managed();
        public static void Init()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            byte[] bytes = Encoding.Unicode.GetBytes(Environment.MachineName + DateTime.Now.Ticks.ToString());
            foreach (NetworkInterface i in interfaces)
            {
                byte[] NewBytes = i.GetPhysicalAddress().GetAddressBytes();
                byte[] AllBytes = new byte[bytes.Length + NewBytes.Length];
                bytes.CopyTo(AllBytes, 0);
                NewBytes.CopyTo(AllBytes, bytes.Length);
                bytes = hasher.ComputeHash(NewBytes);
            }
            ToAdd = bytes;
            rand = new Random(BitConverter.ToInt32(bytes, 0));
        }
        static byte[] GenBytes()
        {
            if (rand == null) Init();
            byte[] RandBytes = new byte[16];
            rand.NextBytes(RandBytes);
            byte[] AllBytes = new byte[RandBytes.Length + ToAdd.Length];
            RandBytes.CopyTo(AllBytes, 0);
            ToAdd.CopyTo(AllBytes, RandBytes.Length);
            return hasher.ComputeHash(AllBytes);
        }
        public static ulong Generate()
        {
            return BitConverter.ToUInt64(GenBytes(), 0);
        }
        public static ulong Generate(ulong max)
        {
            return Generate() % max;
        }
        public static ulong Generate(ulong min, ulong max)
        {
            return Generate(max - min) + min;
        }
    }
}
