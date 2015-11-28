using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Data;

using CDS.Common;

namespace CDS.Data
{
	public class LocalNode : Node
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
		public new string Name
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
        public new string FullName 
        {
            get 
            {
                if (Parent != null)
                {
                    return Parent.FullName + "." + Name;
                }
                else 
                {
                    return Name;
                }
            }
        }
        public new NodeType Type
		{
			get
			{
				return (NodeType)(int)SqliteWrapper.ExecScalar ("SELECT Type FROM Nodes WHERE Id = " + Id.ToString());
			}
		}
		public new LocalNode Parent
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
		public new List<LocalNode> Children
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
		public override void Delete()
		{
			SqliteWrapper.ExecNonQuery ("DELETE FROM Nodes WHERE Id=" + Id.ToString ());
			Id = -1;
		}
		public override CDSData Read()
		{
			switch (Type) 
			{
			case NodeType.Hollow:
				return null;
            case NodeType.Data:
                return CDSData.FromRaw((byte[])SqliteWrapper.ExecScalar("SELECT Contents FROM Nodes WHERE Id = " + Id.ToString()));
			case NodeType.Stream:
				//shitshitshit
				throw new System.NotImplementedException("go directly to prison, do not pass go, do not collect £200");
			default:
				throw new System.NotImplementedException("you have won second prize in a beauty contest. collect £20");
			}
		}
		public override void Write(CDSData Data)
		{
			switch (Type) 
			{
			case NodeType.Hollow:
				break;
            case NodeType.Data:
                SQLiteCommand c = new SQLiteCommand("UPDATE Nodes SET Contents=@Contents WHERE Id =" + Id.ToString());
                c.Parameters.Add("@Contents", DbType.Binary).Value = Data.ToRaw();
                SqliteWrapper.ExecNonQuery(c);
				break;
			case NodeType.Stream:
				//shitshitshit shitshitshit
				throw new System.NotImplementedException("building tax!");
			}
		}
		public override Node AddChild(NodeType type, string Name)
		{
			SQLiteCommand c = new SQLiteCommand("INSERT INTO Nodes (Name, Type, ParentId) VALUES (@Name, @Type, @ParentId)");
			c.Parameters.Add ("@Name", DbType.String).Value = ValidateName (Name);
			c.Parameters.Add ("@Type", DbType.Int32).Value = type;
			c.Parameters.Add ("@ParentId", DbType.Int32).Value = Id;
			SqliteWrapper.ExecNonQuery (c);
            return new LocalNode() { Id = SqliteWrapper.LastInsertRowId };
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
	}
}