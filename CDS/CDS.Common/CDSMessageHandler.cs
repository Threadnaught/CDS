using System;
using System.Collections.Generic;
using System.Linq;

namespace CDS.Common
{
	public class CDSMessageHandler
	{
		//PARSING AND TRANSMITTING CDS MESSAGES FROM AND TO AGENTS
		public Dictionary<int, Agent> Agents = new Dictionary<int, Agent>();
		public ChannelEncap chan;
        public static Dictionary<bool, AgentFactory> agentFactories = new Dictionary<bool,AgentFactory>();
        
        public bool Alive = true;
		public CDSMessageHandler (ChannelEncap channel)
		{
			chan = channel;
			chan.OnChannelCreate += OnChannelCreate;
			chan.OnChannelDelete += OnChannelDelete;
			chan.OnDataReceive += OnDataReceive;
            chan.OnClose += ChanClosed;
		}
		public CDSMessageHandler (System.Net.IPAddress Address)
		{
			System.Net.Sockets.TcpClient c = new System.Net.Sockets.TcpClient ();
			c.Connect (new System.Net.IPEndPoint (Address, 13245));
            TcpMessageEncap e = new TcpMessageEncap(c);
			chan = new ChannelEncap (e);
			chan.OnChannelCreate += OnChannelCreate;
			chan.OnChannelDelete += OnChannelDelete;
			chan.OnDataReceive += OnDataReceive;
            e.Init();
		}
		void OnChannelCreate(int ch)
		{
			//add new Remote agent on local side
			Agents.Add(ch, agentFactories[false].Open(ch, this));
		}
        void ChanClosed()
        {
            Alive = false;
        }
		void OnChannelDelete(int ch)
		{
			Agents.Remove (ch);
		}
		void OnDataReceive(int ch, byte[] Data)
		{
			//parse CDS message
			byte Op = Data[0];
			string TgtNode = "";
			int i;
			for (i = 1; i < Data.Length; i++) 
			{
				if (Data [i] == 0)
					break;
				TgtNode += (char)Data [i];
			}
			int OperationID = (int)BitConverter.ToUInt32(Data, i + 1);
			byte[] Body = new byte[Data.Length - (i + 5)];
			Array.Copy(Data, i + 5 , Body, 0, Body.Length);

			Agents [ch].OnReceiveCDSMessage (Op, TgtNode, OperationID, Body);

		}
		public void SendMessage(int ch, byte Op, string TgtNode, int OpID, byte[] Body)
		{
			//construct CDS message
			byte[] Message = new byte[Body.Length + TgtNode.Length + 6];
			Message [0] = Op;
			System.Text.Encoding.ASCII.GetBytes (TgtNode).CopyTo (Message, 1);
			Message [TgtNode.Length + 2] = 0;
			BitConverter.GetBytes ((UInt32)OpID).CopyTo (Message, TgtNode.Length + 2);
			Body.CopyTo(Message, TgtNode.Length + 6);

			chan.SendMessage (Message, ch);
		}
		public void CloseChannel(Agent a)
		{
			int ID = Agents.FirstOrDefault (x => x.Value == a).Key;
			Agents.Remove (ID);
			chan.DeleteChannel (ID);
		}
	}


}