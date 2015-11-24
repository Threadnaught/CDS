using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Data;

using CDS.Common;

namespace CDS.Server
{
	public class LocalNode
	{
		int Id;
		int ParentId
		{
			get
			{
				return (int)SqliteWrapper.ExecScalar ("SELECT ParentId FROM Nodes WHERE Id = " + Id.ToString());
			}
			set
			{
				SQLiteCommand c = new SQLiteCommand ("UPDATE Nodes SET PatentId=@Contents WHERE Id =" + Id.ToString ());
				c.Parameters.Add ("@Contents", DbType.Int32).Value = value;
				SqliteWrapper.ExecNonQuery (c);
			}
		}
		public string Name
		{
			get
			{
				return (string)SqliteWrapper.ExecScalar ("SELECT Name FROM Nodes WHERE Id = " + Id.ToString());
			}
			set
			{
				SQLiteCommand c = new SQLiteCommand ("UPDATE Nodes SET Name=@Contents WHERE Id =" + Id.ToString ());
				c.Parameters.Add ("@Contents", DbType.String).Value = ValidateName(value);
				SqliteWrapper.ExecNonQuery (c);
			}
		}
		public NodeType Type
		{
			get
			{
				return (NodeType)SqliteWrapper.ExecScalar ("SELECT Type FROM Nodes WHERE Id = " + Id.ToString());
			}
		}
		public LocalNode Parent
		{
			get
			{
				return new LocalNode (){ Id = ParentId };
			}
			set
			{
				ParentId = value.Id;
			}
		}
		public byte[] Data
		{
			get
			{
				return (byte[])SqliteWrapper.ExecScalar ("SELECT Contents FROM Nodes WHERE Id = " + Id.ToString());
			}	
			set
			{
				SQLiteCommand c = new SQLiteCommand ("UPDATE Nodes SET Contents=@Contents WHERE Id =" + Id.ToString ());
				c.Parameters.Add ("@Contents", DbType.Binary).Value = value;
				SqliteWrapper.ExecNonQuery (c);
			}
		}
		public List<LocalNode> Children
		{
			get
			{
				List<LocalNode> Ret = new List<LocalNode> ();
				SQLiteDataReader r = SqliteWrapper.ExecReader ("SELECT Id FROM Nodes WHERE ParentId = " + Id.ToString());
				while (r.Read ()) 
				{
					Ret.Add (new LocalNode (){ Id = r.GetInt32(0) });
				}
				return Ret;
			}
		}
		public void Delete()
		{
			SqliteWrapper.ExecNonQuery ("DELETE FROM Nodes WHERE Id=" + Id.ToString ());
			Id = -1;
		}

		public byte[] Read()
		{
			switch (Type) 
			{
			case NodeType.Hollow:
				return new byte[0];
			case NodeType.Data:
				return Data;
			case NodeType.Stream:
				//shitshitshit
				throw new System.NotImplementedException("go directly to prison, do not pass go, do not collect £200");
			default:
				throw new System.NotImplementedException("you have won second prize in a beauty contest. collect £20");
			}
		}
		public void Write(byte[] data)
		{
			switch (Type) 
			{
			case NodeType.Hollow:
				break;
			case NodeType.Data:
				Data = data;
				break;
			case NodeType.Stream:
				//shitshitshit shitshitshit
				throw new System.NotImplementedException("building tax!");
			}
		}
		public void AddChild(NodeType type, string Name)
		{
			SQLiteCommand c = new SQLiteCommand("INSERT INTO Nodes (Name, Type, ParentId) VALUES (@Name, @Type, @ParentId)");
			c.Parameters.Add ("@Name", DbType.String).Value = ValidateName (Name);
			c.Parameters.Add ("@Type", DbType.Int32).Value = type;
			c.Parameters.Add ("@ParentId", DbType.Int32).Value = Id;
			SqliteWrapper.ExecNonQuery (c);
		}

		public static LocalNode Root
		{
			get
			{
				return new LocalNode () {
					Id = Convert.ToInt32(SqliteWrapper.ExecScalar ("SELECT Id FROM NODES WHERE ParentId = -1"))
				};
			}
		}

		public static LocalNode ByName(string Name)
		{
			SQLiteCommand c = new SQLiteCommand ("SELECT Id FROM Nodes WHERE Name = @Name");
			c.Parameters.Add ("@Name", DbType.String).Value = ValidateName(Name);
			return new LocalNode (){ Id = Convert.ToInt32(SqliteWrapper.ExecScalar(c)) };
		}
		public static LocalNode Resolve(string Name)
		{
			string[] sections = Name.Split ('.');
			LocalNode n = Root; 
			foreach (string s in sections) 
			{
				if(s != "")
				foreach(LocalNode c in n.Children)
				{
					if(ValidateName(s) == c.Name)
					{
						n = c;
						break;
					}
				}
			}
			return n;
		}
		public static bool ValidName(string s)
		{
			return System.Text.RegularExpressions.Regex.IsMatch (s, "^[a-zA-Z0-9\\-_]*$");
		}
		public static string ValidateName(string s)
		{
			return s.ToLower ();
		}
		public static LocalNode Create(LocalNode Parent, string Name, NodeType Type, byte[] Contents)
		{
			return new LocalNode (){ Id = Create(Parent.Id, Name, Type, Contents)  };
		}
		public static int Create(int ParentId, string Name, NodeType Type, byte[] Contents)
		{
			if (!ValidName (Name)) throw new ArgumentException ("Invalid chars in name");
			SQLiteCommand c = new SQLiteCommand ("INSERT INTO Nodes (ParentId, Name, Type, Contents) VALUES (@ParentId, @Name, @Type, @Contents)");
			c.Parameters.Add ("@ParentId", DbType.Int32).Value = ParentId;
			c.Parameters.Add ("@Name", DbType.String).Value = ValidateName(Name);
			c.Parameters.Add ("@Type", DbType.Int32).Value = (int)Type;
			c.Parameters.Add ("@Contents", DbType.Binary).Value = Contents;

			SqliteWrapper.ExecNonQuery (c);
			return SqliteWrapper.LastInsertRowId;
		}

		public override bool Equals (object obj)
		{
			if (obj.GetType () != GetType ())
				return false;
			return Id == ((LocalNode)obj).Id;
		}
		public static bool operator  == (LocalNode a, LocalNode b)
		{
			return a.Equals (b);
		}
		public static bool operator  != (LocalNode a, LocalNode b)
		{
			return !a.Equals (b);
		}
		public override int GetHashCode ()
		{
			return Id;
		}
		public override string ToString ()
		{
			return Name + "(" + Id.ToString() + ")";
		}
	}
}

