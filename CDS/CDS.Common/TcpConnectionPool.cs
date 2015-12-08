using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

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

        }
        public void SendToOtherSideRaw(byte[] Message)
        {
            TcpConnectionPool.SendMessageToOtherSide(target, Id, Message);
        }
    }
}
