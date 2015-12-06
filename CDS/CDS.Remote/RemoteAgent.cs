using System;
using System.Linq;
using System.Collections.Generic;

using CDS.Common;

namespace CDS.Remote
{
	public class CDSRemoteAgent : Agent
	{
		List<SentOp> SentOps = new List<SentOp>();
		//the side which initiated the channel (sends operations to remote agent)
		//parsing responses, sending commands
		public int ChannelID;
		public override void OnReceiveCDSMessage (byte Op, string TgtNode, int OpID, byte[] Body)
		{
			SentOp ToRemove = null;
			//Response to earilier request
			foreach(SentOp Sent in SentOps)
			{
				if (Sent.OpID == OpID) 
				{
					Sent.response = (CDSResponses)Op;
					Sent.Reply = Body;
					Sent.OnReply (Sent);
					ToRemove = Sent;
					break;
				}
			}
			SentOps.Remove (ToRemove);
		}
		public SentOp SendRequest(CDSOperations Op, string TgtNode, byte[] Body)
		{
            if (CDSHandler.Alive)
            {
                SentOp ret = null;
                SendRequest(Op, TgtNode, Body, (SentOp s) => { ret = s; });
                while (ret == null) System.Threading.Thread.Sleep(10);
                return ret;
            }
            else 
            {
                throw new Exception("connection closed");
            }
		}
		public void SendRequest(CDSOperations Op, string TgtNode, byte[] Body, Reply OnReply)
		{
            if (CDSHandler.Alive)
            {
                int OpID = 0;
                foreach (SentOp o in SentOps)
                    OpID = Math.Max(OpID, o.OpID + 1);
                SentOp NewSentOp = new SentOp() { OpID = OpID };
                NewSentOp.OnReply += OnReply;
                SentOps.Add(NewSentOp);
                CDSHandler.SendMessage(ChannelID, (byte)Op, TgtNode, OpID, Body);
            }
            else
            {
                throw new Exception("connection closed");
            }
		}
		public RemoteNode Root
		{
			get
			{
				return new RemoteNode (this, "");
			}
		}
	}
}
