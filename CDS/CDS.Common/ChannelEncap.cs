using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CDS.Common
{
	public class ChannelEncap
	{
		//MULTIPLEXING DIFFERENT CHANNELS INTO ONE MESSAGE PROVIDER

		public MessageEncap MessageProvider;
		public ChannelCmd OnChannelCreate;
		public ChannelCmd OnChannelDelete;
		public ChannelData OnDataReceive;
        public Close OnClose;
        public bool Alive = true;

		public ChannelEncap (MessageEncap message)
		{
			MessageProvider = message;
			MessageProvider.OnReceiveMessage += MessageFromProvider;
            MessageProvider.OnClose += MessageProviderClosed;
		}
        void MessageProviderClosed() 
        {
            Alive = false;
            OnClose();
        }
		void MessageFromProvider(byte[] Message)
		{
            if (Message.Length != 0)
            {
                byte[] Body = new byte[Message.Length - 5];
                Array.Copy(Message, 5, Body, 0, Body.Length);
                int Channel = (int)BitConverter.ToUInt32(Message, 0);
                switch ((ChannelOp)Message[4])
                {
                    case ChannelOp.Create:
                        OnChannelCreate(Channel);
                        break;
                    case ChannelOp.Delete:
                        OnChannelDelete(Channel);
                        break;
                    case ChannelOp.Message:
                        OnDataReceive(Channel, Body);
                        break;
                }
            }
		}
		public void SendMessage(byte[] Message, int Channel)
		{
			byte[] FullMessage = new byte[Message.Length + 5];
			BitConverter.GetBytes ((UInt32)Channel).CopyTo (FullMessage, 0);
			FullMessage [4] = (byte)ChannelOp.Message;
			Message.CopyTo (FullMessage, 5);
			MessageProvider.SendMessage (FullMessage);
		}

		public void CreateChannel (int Channel)
		{
			byte[] FullMessage = new byte[5];
			BitConverter.GetBytes ((UInt32)Channel).CopyTo (FullMessage, 0);
			FullMessage [4] = (byte)ChannelOp.Create;
			MessageProvider.SendMessage (FullMessage);
		}

		public void DeleteChannel (int Channel)
		{
			byte[] FullMessage = new byte[5];
			BitConverter.GetBytes ((UInt32)Channel).CopyTo (FullMessage, 0);
			FullMessage [4] = (byte)ChannelOp.Delete;
			MessageProvider.SendMessage (FullMessage);
		}
	}
	public delegate void ChannelCmd(int Channel);
	public delegate void ChannelData(int Channel, byte[] Data);
	
}

