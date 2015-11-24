using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace CDS.Common
{
	public static class TcpConnectionListener
	{
        //LISTENING FOR TCP CONNECTIONS AND CREATING HANDLERS FOR THEM
        public static AgentFactory AgentCreator;
		public static List<CDSMessageHandler> AcceptedConnections = new List<CDSMessageHandler> ();
		static TcpListener listener;
		public static bool Alive
		{
			get{ return AliveInternal; }
			set
			{
				foreach (CDSMessageHandler h in AcceptedConnections) 
				{
					h.chan.MessageProvider.Alive = value;
				}
				AliveInternal = value;
			}
		}
		static bool AliveInternal = true;

		public static void Init()
		{
			listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 13245);
			listener.Start ();
			Alive = true;
			new Thread (ListenThread).Start ();
		}
		static void ListenThread()
		{
			while (Alive) 
			{
				if (listener.Pending ()) 
				{
					Console.WriteLine ("Adding connection");
					TcpMessageEncap m = new TcpMessageEncap (listener.AcceptTcpClient ());
					ChannelEncap c = new ChannelEncap (m);
                    CDSMessageHandler h = new CDSMessageHandler(c, AgentCreator);
					AcceptedConnections.Add (h);
				}
				for (int i = 0; i < AcceptedConnections.Count; i++) 
				{
					if (!AcceptedConnections [i].chan.MessageProvider.Alive) 
					{
						Console.WriteLine ("Removing connnection");
						AcceptedConnections.RemoveAt (i);
						i--;
					}
				}
				Thread.Sleep (50);
			}
		}
	}
}

