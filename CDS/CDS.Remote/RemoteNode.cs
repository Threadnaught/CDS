using System;
using System.Collections.Generic;
using System.Linq;

using CDS.Common;

namespace CDS.Remote
{
	public class RemoteNode : Node
	{
		public CDSRemoteAgent agent;
        public RemoteNode(CDSRemoteAgent Agent, string Name)
        {
            FullName = Name;
            agent = Agent;
        }

        public string Name 
        {
            get 
            {
                return FullName.Substring(FullName.Length - (FullName.Split('.').Last().Length + 1));
            }
        }
        public NodeType Type
        {
            get
            {
                return (NodeType)agent.SendRequest(CDSOperations.getType, FullName, new byte[0]).Reply[0];
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
        public new List<RemoteNode> Children
        {
            get
            {
                byte[] bs = agent.SendRequest(CDSOperations.getChildren, FullName, new byte[] { }).Reply;
                List<string> strs = new List<string>();
                strs.Add("");
                foreach (byte b in bs)
                {
                    if (b != 0)
                    {
                        strs[strs.Count - 1] += (char)b;
                    }
                    else
                    {
                        strs.Add("");
                    }
                }
                strs.RemoveAt(strs.Count - 1);
                List<RemoteNode> rs = new List<RemoteNode>();
                foreach (string s in strs)
                {
                    rs.Add(new RemoteNode(agent, FullName + "." + s));
                }
                return rs;
            }
        }
        public override void Delete()
        {
            agent.SendRequest(CDSOperations.delete, FullName, new byte[] { });
        }
		public override CDSData Read()
		{
			SentOp o = agent.SendRequest(CDSOperations.read, FullName, new byte[]{});
			return CDSData.FromRaw(o.Reply);
		}
		public override void Write(CDSData Data)
		{
			agent.SendRequest (CDSOperations.write, FullName, Data.ToRaw());
		}
        public override Node AddChild(NodeType type, string Name)
        {
            agent.SendRequest(CDSOperations.create, FullName + "." + Name, new byte[] { (byte)type });
            return new RemoteNode(agent, FullName + "." + Name);
        }
	}
}