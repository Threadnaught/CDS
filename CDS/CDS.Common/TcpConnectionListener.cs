using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CDS.Common
{
	public static class TcpConnectionListener
	{
        //LISTENING FOR TCP CONNECTIONS AND CREATING HANDLERS FOR THEM
        public static Dictionary<bool, AgentFactory> AgentCreators = new Dictionary<bool,AgentFactory>();
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
            Task.Factory.StartNew(Listen);
        }
        static void Listen()
        {
            if (listener.Pending())
            {
                Console.WriteLine("Adding connection");
                TcpMessageEncap m = new TcpMessageEncap(listener.AcceptTcpClient());
                ChannelEncap c = new ChannelEncap(m);
                CDSMessageHandler h = new CDSMessageHandler(c);
                AcceptedConnections.Add(h);
                m.Init();
            }
            for (int i = 0; i < AcceptedConnections.Count; i++)
            {
                if (!AcceptedConnections[i].chan.MessageProvider.Alive)
                {
                    Console.WriteLine("Removing connnection");
                    AcceptedConnections.RemoveAt(i);
                    i--;
                }
            }
            Task.Factory.StartNew(Listen);
            
        }
	}
}

