using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CDS.Common
{
	public abstract class MessageEncap
	{
		//ABSTRACT MESSAGE PROVIDER
		public ReceiveMessage OnReceiveMessage;
		public abstract void SendMessage (byte[] Msg);
		public bool Alive = true;
	}
	public class TcpMessageEncap : MessageEncap
	{
		//VERY SIMPLE PROTOCOL TO WRAP TCP BUFFERS INTO DISCRETE MESSAGES
		//EVERY MESSAGE LEADS WITH A 4 BYTE LENGTH (UInt32)

		TcpClient c;

		public TcpMessageEncap (TcpClient client)
		{
			c = client;
		}
        public void Init() 
        {
            new Thread(ReceiveMessageThread).Start();
        }
		public override void SendMessage(byte[] Msg)
		{
			c.GetStream ().Write (BitConverter.GetBytes((UInt32)Msg.Length), 0, 4);
			c.GetStream ().Write (Msg, 0, Msg.Length);
		}
		void ReceiveMessageThread()
		{
			while (Alive) 
			{
				if (c.Available >= 4) 
				{
					byte[] LengthBuffer = new byte[4];
					c.GetStream ().Read (LengthBuffer, 0, 4);

					byte[] MessageBuffer = new byte[BitConverter.ToUInt32(LengthBuffer, 0)];
					c.GetStream ().Read (MessageBuffer, 0, MessageBuffer.Length);

					OnReceiveMessage (MessageBuffer);

					//TODO: add timeout
				} 
				else 
				{
					Thread.Sleep (50);
				}
				try
				{
					c.GetStream().Write(new byte[]{}, 0, 0);
				}
				catch
				{
					Alive = false;
				}
			}
		}
	}
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

