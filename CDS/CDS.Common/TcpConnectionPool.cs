using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Sockets;

namespace CDS.Common
{
    public static class TcpConPool
    {
        public static TimeSpan ConnectionExpireTime = new TimeSpan(0, 5, 0);
        public static Dictionary<IPAddress, Connection> Connections = new Dictionary<IPAddress, Connection>();
    }
    public class Connection
    {
        public IPAddress target;
        TcpClient client;
        public DateTime ClientExpires;
        public Dictionary<ulong, Channel> Channels = new Dictionary<ulong, Channel>();
        public Dictionary<ulong, DateTime> ChannelsExpire = new Dictionary<ulong, DateTime>();
        public void NewClient(TcpClient cli)
        {
            lock (this)
            {
                if (client == null)
                {
                    client = cli;
                    ClientExpires = DateTime.Now + TcpConPool.ConnectionExpireTime;
                    Task.Factory.StartNew(ConnectionTask);
                }
                else
                {
                    client = cli;
                    ClientExpires = DateTime.Now + TcpConPool.ConnectionExpireTime;
                }
            }
        }
        void ConnectionTask()
        {
            lock (this)
            {
                Stream s = client.GetStream();
                if (client == null) return;
                //checking if there is an incoming message:
                if (client.Available >= 4)
                {
                    //message ahoy!
                    ulong Length = BitConverter.ToUInt64(s.ReadBytesFromStream(8), 0);
                    MessageType type = (MessageType)s.ReadByte();
                    if ((byte)type > 127)
                    {
                        //response type
                        ulong MessageID = BitConverter.ToUInt64(s.ReadBytesFromStream(8), 0);
                        foreach (Channel c in Channels.Values)
                        {
                            if (c.MessagesAwaiting.Keys.Contains(MessageID))
                            {
                                c.MessagesAwaiting[MessageID].ReceiveResponse(s, Length - 1, type);
                                break;
                            }
                        }
                    }
                    else
                    {
                        //request type

                    }
                    ClientExpires = DateTime.Now + TcpConPool.ConnectionExpireTime;
                }
                //checking if the client has expired:
                if (ClientExpires > DateTime.Now)
                {
                    client.Close();
                    client = null;
                    return;
                }
                //checking if the client was closed:
                try
                {
                    s.Write(new byte[0], 0, 0);
                }
                catch
                {
                    client.Close();
                    client = null;
                    return;
                }
                //scheduling the next run through
                Task.Factory.StartNew(ConnectionTask);
            }
        }
    }
    public class Channel
    {
        public Dictionary<ulong, Request> MessagesAwaiting = new Dictionary<ulong, Request>();
        public void ReceiveRequest(Stream request, ulong Length, MessageType type)
        {

        }
    }
    public class Request
    {
        public void ReceiveResponse(Stream response, ulong Length, MessageType type)
        {

        }
    }
    public static class Utils
    {
        public static byte[] ReadBytesFromStream(this Stream s, int Len)
        {
            byte[] Ret = new byte[Len];
            s.Read(Ret, 0, Len);
            return Ret;
        }
    }
    public enum MessageType : byte
    {
        read = 0,
        write = 1,
        create = 2,
        delete = 3,
        getType = 4,
        getChildren = 5,
        nodeExists = 6,
        subscribe = 7,
        unsubscribe = 8,
        resetConnection = 9,

        subscriberUpdate = 253,
        requestSuccessful = 254,
        requestFailed = 255,
    }
}
