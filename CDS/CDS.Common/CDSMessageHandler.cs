using System;
using System.Collections.Generic;
using System.Linq;

namespace CDS.Common
{
	public class CDSMessageHandler
	{
		//PARSING AND TRANSMITTING CDS MESSAGES FROM AND TO AGENTS
		public Dictionary<int, CDSAgent> Agents = new Dictionary<int, CDSAgent>();
		public ChannelEncap chan;
        public AgentFactory agentCreator;
		public CDSMessageHandler (ChannelEncap channel, AgentFactory f)
		{
			chan = channel;
			chan.OnChannelCreate += OnChannelCreate;
			chan.OnChannelDelete += OnChannelDelete;
			chan.OnDataReceive += OnDataReceive;
            agentCreator = f;
		}
		public CDSMessageHandler (System.Net.IPAddress Address, AgentFactory f)
		{
			System.Net.Sockets.TcpClient c = new System.Net.Sockets.TcpClient ();
			c.Connect (new System.Net.IPEndPoint (Address, 13245));
			chan = new ChannelEncap (new TcpMessageEncap (c));
			chan.OnChannelCreate += OnChannelCreate;
			chan.OnChannelDelete += OnChannelDelete;
			chan.OnDataReceive += OnDataReceive;
            agentCreator = f;
		}
		void OnChannelCreate(int ch)
		{
			//add new Remote agent on local side
			Agents.Add(ch, agentCreator.Open(false, ch, this));
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
		public CDSAgent OpenNewChannel()
		{
			int ID = 0;
			foreach(int NewID in Agents.Keys)
			{
				if (ID <= NewID)
					ID = NewID + 1;
			}
			chan.CreateChannel (ID);
            Agents.Add(ID, agentCreator.Open(false, ID, this));
			return Agents[ID];
		}
		public void CloseChannel(CDSAgent a)
		{
			int ID = Agents.FirstOrDefault (x => x.Value == a).Key;
			Agents.Remove (ID);
			chan.DeleteChannel (ID);
		}
	}


}

