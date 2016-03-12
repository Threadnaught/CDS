﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace CDS.Communication
{
    public class TcpConnection : Connection
    {
        TcpClient c;
        public const int Port = 12345;
        public TcpConnection(IPAddress target)
        {
            c = new TcpClient();
            c.Connect(target, Port);
        }
        public TcpConnection(TcpClient cli)
        {
            c = cli;
        }
        public override void SendMessage(byte[] Message)
        {
            try
            {
                byte[] Len = BitConverter.GetBytes((ulong)Message.Length);
                c.GetStream().Write(Len, 0, 8);
                c.GetStream().Write(Message, 0, Message.Length);
            }
            catch
            {
                Closed();
            }
        }
        public override bool Closed()
        {
            if (Expired || !c.Connected) return true;
            try
            {
                c.GetStream().Write(new byte[0], 0, 0);
            }
            catch
            {
                return true;
            }
            return false;
        }
        protected override Stream GetStream()
        {
            return c.GetStream();
        }
        protected override bool MessageIncoming()
        {
            //is a message incoming
            try
            {
                return c.GetStream().DataAvailable;
            }
            catch
            {
                Closed();
                return false;
            }
        }
        protected override ulong MessageLength()
        {
            //length of incoming message
            try
            {
                byte[] Length = new byte[8];
                c.GetStream().Read(Length, 0, 8);
                return BitConverter.ToUInt64(Length, 0);
            }
            catch
            {
                Closed();
                return 0;
            }
        }
    }
}