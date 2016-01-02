﻿using System;
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
        public static TimeSpan ConnectionExpireTime = new TimeSpan(0, 10, 0);
        public static Dictionary<IPAddress, Connection> Connections = new Dictionary<IPAddress, Connection>();
    }
    public class Connection
    {
        public IPAddress target;
        TcpClient client;
        public DateTime ClientExpires;
        public Dictionary<UInt64, Channel> Channels = new Dictionary<UInt64, Channel>();
        public Dictionary<UInt64, DateTime> ChannelsExpire = new Dictionary<UInt64, DateTime>();
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
                    UInt64 Length = BitConverter.ToUInt64(s.ReadBytesFromStream(8), 0);
                    MessageType type = (MessageType)s.ReadByte();
                    if ((byte)type > 127)
                    {
                        //response type
                        UInt64 MessageID = BitConverter.ToUInt64(s.ReadBytesFromStream(8), 0);
                        foreach (Channel c in Channels.Values)
                        {
                            if (c.MessagesAwaiting.Keys.Contains(MessageID))
                            {
                                c.MessagesAwaiting[MessageID].ReceiveResponse(s, Length - 1);
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
        public Dictionary<UInt64, Request> MessagesAwaiting = new Dictionary<UInt64, Request>();
    }
    public class Request
    {
        public void ReceiveResponse(Stream response, UInt64 Length)
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
        public static byte[] GenNoncollidingBits(int len)
        {
            byte[] ret = new byte[len];
            Array.Copy(Guid.NewGuid().ToByteArray(), ret, len);
            return ret;
        }
        public static UInt64 GenNoncollidingUint()
        {
            return BitConverter.ToUInt64(GenNoncollidingBits(8), 0);
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
