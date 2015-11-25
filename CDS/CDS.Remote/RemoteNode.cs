using System;
using System.Collections.Generic;
using System.Linq;

using CDS.Common;

namespace CDS.Remote
{
	public class RemoteNode
	{
		public CDSLocalAgent agent;
		public RemoteNode parent;
		public string FullName;
		public RemoteNode(CDSLocalAgent Agent, string Name)
		{
			FullName = Name;
			agent = Agent;
		}
		public byte[] Read()
		{
			SentOp o = agent.SendRequest(CDSOperations.read, FullName, new byte[]{});
			return o.Reply;
		}
		public void Write(byte[] Data)
		{
			agent.SendRequest (CDSOperations.write, FullName, Data);
		}
		public NodeType Type
		{
			get
			{
				return (NodeType)agent.SendRequest (CDSOperations.getType, FullName, new byte[0]).Reply [0];
			}
		}
		public RemoteNode Parent
		{
			get
			{
				if (FullName == "")
					return null;
				return new RemoteNode(agent, FullName.Substring (0, FullName.Length - (FullName.Split ('.').Last ().Length + 1)));
			}
		}
		public List<RemoteNode> Children
		{
			get
			{
				byte[] bs = agent.SendRequest (CDSOperations.getChildren, FullName, new byte[]{ }).Reply;
				List<string> strs = new List<string> ();
				strs.Add ("");
				foreach (byte b in bs) 
				{
					if (b != 0) 
					{
						strs [strs.Count - 1] += (char)b;
					}
					else 
					{
						strs.Add ("");
					}
				}
				strs.RemoveAt (strs.Count - 1);
				List<RemoteNode> rs = new List<RemoteNode> ();
				foreach (string s in strs) 
				{
					rs.Add (new RemoteNode (agent, FullName + "." + s));
				}
				return rs;
			}
		}
		public RemoteNode CreateChild(string ChildName, NodeType t)
		{
			agent.SendRequest (CDSOperations.create, FullName + "." + ChildName, new byte[]{ (byte)t });
			return new RemoteNode (agent, FullName + "." + ChildName);
		}
		public void Delete()
		{
			agent.SendRequest (CDSOperations.delete, FullName, new byte[]{ });
		}
	}
}