using System;
using System.Linq;
using System.Collections.Generic;
using CDS.Common;

namespace CDS.Data
{
	public class CDSLocalAgent : CDSAgent
	{
		//the side which received the channel (sends responses back to local agent)
		//parsing commands, sending responses
		public int ChannelID;
		public override void OnReceiveCDSMessage (byte Op, string TgtNode, int OpID, byte[] Body)
		{
			//apply CDSOperation specified by Op and send response
			LocalNode tgt;
			try
			{
				switch ((CDSOperations)Op) 
				{
				case CDSOperations.read:
					tgt = LocalNode.Resolve (TgtNode);
					CDSHandler.SendMessage (ChannelID, (byte)CDSResponses.Success, TgtNode, OpID, tgt.Read ().ToRaw());
					break;
				case CDSOperations.write:
					tgt = LocalNode.Resolve (TgtNode);
					tgt.Write (CDSData.FromRaw(Body));
					CDSHandler.SendMessage(ChannelID, (byte)CDSResponses.Success, TgtNode, OpID, new byte[0]);
					break;
				case CDSOperations.create:
					tgt = LocalNode.Resolve(TgtNode.Substring(0, TgtNode.Length - (TgtNode.Split('.').Last().Length + 1)));
					tgt.AddChild((NodeType)Body[0], TgtNode.Split('.').Last());
					CDSHandler.SendMessage(ChannelID, (byte)CDSResponses.Success, TgtNode, OpID, new byte[0]);
					break;
				case CDSOperations.delete:
					tgt = LocalNode.Resolve (TgtNode);
					tgt.Delete ();
					CDSHandler.SendMessage(ChannelID, (byte)CDSResponses.Success, TgtNode, OpID, new byte[0]);
					break;
				case CDSOperations.getType:
					tgt = LocalNode.Resolve (TgtNode);
					CDSHandler.SendMessage(ChannelID, (byte)CDSResponses.Success, TgtNode, OpID, new byte[]{(byte)tgt.Type()});
					break;
				case CDSOperations.getChildren:
					tgt = LocalNode.Resolve (TgtNode);
					string Ret = "";
					foreach(LocalNode c in tgt.Children())
					{
						Ret += c.Name();
						Ret += (char)0;
					}
					CDSHandler.SendMessage(ChannelID, (byte)CDSResponses.Success, TgtNode, OpID, System.Text.Encoding.ASCII.GetBytes(Ret));
					break;
				case CDSOperations.subscribe:
					tgt = LocalNode.Resolve (TgtNode);
					CDSHandler.SendMessage(ChannelID, (byte)CDSResponses.Failure, TgtNode, OpID, new byte[0]);
					break;
				case CDSOperations.unsubscribe:
					tgt = LocalNode.Resolve (TgtNode);
					CDSHandler.SendMessage(ChannelID, (byte)CDSResponses.Failure, TgtNode, OpID, new byte[0]);
					break;
				}
			}
			catch
			{
				CDSHandler.SendMessage(ChannelID, (byte)CDSResponses.Failure, TgtNode, OpID, new byte[0]);
			}
		}
	}
}