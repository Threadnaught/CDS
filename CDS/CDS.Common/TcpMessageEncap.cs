﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CDS.Common
{
    public abstract class MessageEncap
    {
        //ABSTRACT MESSAGE PROVIDER
        public ReceiveMessage OnReceiveMessage;
        public Close OnClose;
        public abstract void SendMessage(byte[] Msg);
        public bool Alive = true;
    }
    public class TcpMessageEncap : MessageEncap
    {
        //VERY SIMPLE PROTOCOL TO WRAP TCP BUFFERS INTO DISCRETE MESSAGES
        //EVERY MESSAGE LEADS WITH A 4 BYTE LENGTH (UInt32)
        TcpClient c;
        NetworkStream stream;
        public static List<TcpMessageEncap> Encaps = new List<TcpMessageEncap>();
        bool ReceivingMessage = false;
        public TcpMessageEncap(TcpClient client)
        {
            OnClose += () => Encaps.Remove(this); 
            Encaps.Add(this);
            c = client;
            stream = c.GetStream();
        }
        public void Init()
        {
            Task.Factory.StartNew(ReceiveMessage);
        }
        public override void SendMessage(byte[] Msg)
        {
            try
            {
                stream.Write(BitConverter.GetBytes((UInt32)Msg.Length), 0, 4);
                stream.Write(Msg, 0, Msg.Length);
            }
            catch
            {
                Alive = false;
                c.Close();
                OnClose();
            }
        }
        void ReceiveMessage()
        {
            if (c.Available >= 4 && !ReceivingMessage)
            {
                ReceivingMessage = true;
                Task.Factory.StartNew(MessageIncoming);
            }
            try
            {
                stream.Write(new byte[] { }, 0, 0);
            }
            catch
            {
                Alive = false;
                c.Close();
                OnClose();
            }
            if (Alive) Task.Factory.StartNew(ReceiveMessage);
        }
        void MessageIncoming()
        {
            byte[] LengthBuffer = new byte[4];
            stream.Read(LengthBuffer, 0, 4);
            DateTime Start = DateTime.Now;
            while (c.Available < BitConverter.ToUInt32(LengthBuffer, 0) && (DateTime.Now - Start).TotalSeconds < 5) { }
            if ((DateTime.Now - Start).TotalSeconds < 5)
            {
                byte[] MessageBuffer = new byte[BitConverter.ToUInt32(LengthBuffer, 0)];
                stream.Read(MessageBuffer, 0, MessageBuffer.Length);
                OnReceiveMessage(MessageBuffer);
            }
            else
            {
                Alive = false;
                c.Close();
                OnClose();
            }
            ReceivingMessage = false;
        }
    }
    public delegate void Close();
    public delegate void ReceiveMessage(byte[] Message);
    public class SentOp
    {
        public int OpID;
        public CDSResponses response;
        public byte[] Reply;
        public Reply OnReply;
    }
    public delegate void Reply(SentOp s);
}