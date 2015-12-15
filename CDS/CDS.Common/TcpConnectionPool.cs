using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace CDS.Common
{
    public static class TcpConnectionPool
    {
        public static Dictionary<IPAddress, TcpMessageEncap> Messages = new Dictionary<IPAddress, TcpMessageEncap>();
        public static Dictionary<ulong, Channel> Channels = new Dictionary<ulong, Channel>();
        public static void SendMessageToOtherSide(IPAddress target, ulong ChannelId, byte[] message)
        {
            if (!Messages.ContainsKey(target))
            {
                Messages.Add(target, new TcpMessageEncap(target));
            }
            byte[] FullMessage = new byte[message.Length + 8];
            BitConverter.GetBytes(ChannelId).CopyTo(FullMessage, 0);
            message.CopyTo(FullMessage, 8);
            Messages[target].SendMessage(FullMessage);
        }
        public static void ReceiveMessageFromOtherSide(IPAddress from, byte[] message)
        {
            ulong ChannelId = BitConverter.ToUInt64(message, 0);
            if (!Channels.ContainsKey(ChannelId))
            {
                Channels.Add(ChannelId, new Channel() { Id = ChannelId, target = from });
            }
            byte[] Body = new byte[message.Length - 8];
            Array.Copy(message, 8, Body, 0, Body.Length);
            Channels[ChannelId].ReceiveFromOtherSideRaw(Body);
        }
    }
    public class Channel
    {
        public ulong Id;
        public IPAddress target;
        public void ReceiveFromOtherSideRaw(byte[] Message)
        {
            CDSMessage message = CDSMessage.FromStream(new MemoryStream(Message));
        }
    }
    public struct CDSMessage
    {
        public UInt32 ChannelId;
        public ChannelOperation ChannelOp;

        public byte CDSOp;
        public string TargetNode;
        public UInt32 Seq;
        public byte[] Message;

        public MemoryStream ToStream()
        {
            MemoryStream s = new MemoryStream();
            s.Write(BitConverter.GetBytes(ChannelId), 0, 4);
            s.Write(BitConverter.GetBytes((byte)ChannelOp), 0, 1);
            s.Write(BitConverter.GetBytes(CDSOp), 0, 1);
            foreach (byte b in Encoding.ASCII.GetBytes(TargetNode)) s.WriteByte(b);
            s.WriteByte(0);
            s.Write(BitConverter.GetBytes(Seq), 0, 4);
            s.Write(Message, 0, Message.Length);
            return s;
        }
        public static CDSMessage FromStream(MemoryStream s)
        {
            CDSMessage m = new CDSMessage();
            m.LoadStream(s);
            return m;
        }
        void LoadStream(MemoryStream s)
        {
            s.Seek(0, SeekOrigin.Begin);
            ChannelId = BitConverter.ToUInt32(s.ReadBytes(4), 0);
            ChannelOp = (ChannelOperation)s.ReadByte();
            CDSOp = (byte)s.ReadByte();
            TargetNode = "";
            int Last = s.ReadByte();
            while (Last != 0)
            {
                TargetNode += (char)Last;
                Last = s.ReadByte();
            }
            Seq = BitConverter.ToUInt32(s.ReadBytes(4), 0);
            Message = s.ReadBytes((int)(s.Length - s.Position));
        }
    }
    static class StreamUtils
    {
        public static byte[] ReadBytes(this MemoryStream s, int Len)
        {
            byte[] Ret = new byte[Len];
            s.Read(Ret, 0, Len);
            return Ret;
        }
    }
    public enum ChannelOperation : byte
    {
        RemoteToLocal = 0,
        LocalToRemote = 1,
    }
}
