using System;
using System.Collections.Generic;
using System.Linq;

using CDS.Common;

namespace CDS.Remote
{
    public class RemoteNode : Node
    {
        public CDSRemoteAgent agent;
        string fullName;
        public RemoteNode(CDSRemoteAgent Agent, string Name)
        {
            fullName = Name;
            agent = Agent;
        }
        public override string GetFullName()
        {
            return fullName;
        }
        public override string GetName()
        {
            return fullName.Substring(fullName.Length - (fullName.Split('.').Last().Length));
        }
        public override NodeType GetNodeType()
        {
            return (NodeType)agent.SendRequest(CDSOperations.getType, fullName, new byte[0]).Reply[0];
        }
        public override Node GetParent()
        {
            if (fullName == "")
                return null;
            return new RemoteNode(agent, fullName.Substring(0, fullName.Length - (fullName.Split('.').Last().Length + 1)));
        }
        public override List<Node> GetChildren()
        {
            byte[] bs = agent.SendRequest(CDSOperations.getChildren, fullName, new byte[] { }).Reply;
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
            List<Node> rs = new List<Node>();
            foreach (string s in strs)
            {
                rs.Add(new RemoteNode(agent, fullName + "." + s));
            }
            return rs;
        }
        public override void Delete()
        {
            agent.SendRequest(CDSOperations.delete, fullName, new byte[] { });
        }
        public override CDSData Read()
        {
            SentOp o = agent.SendRequest(CDSOperations.read, fullName, new byte[] { });
            return CDSData.FromRaw(o.Reply);
        }
        public override void Write(CDSData Data)
        {
            agent.SendRequest(CDSOperations.write, fullName, Data.ToRaw());
        }
        public override Node AddChild(NodeType type, string Name)
        {
            agent.SendRequest(CDSOperations.create, fullName + "." + Name, new byte[] { (byte)type });
            return new RemoteNode(agent, fullName + "." + Name);
        }
        public override bool GetIfExists()
        {
            SentOp o = agent.SendRequest(CDSOperations.nodeExists, fullName, new byte[] { });
            return BitConverter.ToBoolean(o.Reply, 0);
        }
    }
}