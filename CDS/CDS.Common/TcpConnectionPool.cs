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
        public static TimeSpan ConnectionExpireTime = new TimeSpan(0, 10, 0);
        public static Dictionary<IPAddress, Connection> Connections = new Dictionary<IPAddress, Connection>();
    }
    public class Connection
    {
        public IPAddress target;
        TcpClient client;
        public DateTime ClientExpires;
        public Dictionary<UInt32, Channel> Channels = new Dictionary<uint, Channel>();
        public Dictionary<UInt32, DateTime> ChannelsExpire = new Dictionary<uint, DateTime>();
        public void NewClient(TcpClient cli)
        {
            client = cli;
            ClientExpires = DateTime.Now + TcpConPool.ConnectionExpireTime;
            Task.Factory.StartNew(ConnectionTask);
        }
        void ConnectionTask()
        {
            if (client.Available >= 4)
            {
                //message ahoy!
                ClientExpires = DateTime.Now + TcpConPool.ConnectionExpireTime;
            }
            if (ClientExpires > DateTime.Now)
            {
                client.Close();
                client = null;
                return;
            }
            try
            {
                client.GetStream().Write(new byte[0], 0, 0);
            }
            catch
            {
                client.Close();
                client = null;
                return;
            }
            Task.Factory.StartNew(ConnectionTask);
        }
    }
    public class Channel
    {

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
