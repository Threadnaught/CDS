using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Data;

using CDS.Common;

namespace CDS.Server
{
	public static class SqliteWrapper
	{
		static SQLiteConnection c;
		public static int LastInsertRowId
		{
			get
			{
				return Convert.ToInt32(c.LastInsertRowId);
			}
		}
		public static void Init()
		{
			if (!File.Exists ("Nodes.cds")) 
			{
				//file initalisation
				SQLiteConnection.CreateFile ("Nodes.cds");
				c = new SQLiteConnection ("Data Source=Nodes.cds;Version=3;");
				c.Open ();
				ExecNonQuery("CREATE TABLE Nodes (Id INTEGER PRIMARY KEY AUTOINCREMENT, ParentId INT, Name VARCHAR(32), Type INT, Contents BLOB)");
				LocalNode.Create (-1, "Root", NodeType.Hollow, new byte[]{ });
			}
			else 
			{
				//file already created
				c = new SQLiteConnection ("Data Source=Nodes.cds;Version=3;");
				c.Open ();
			}
		}
		public static void ExecNonQuery(string Query)
		{
			new SQLiteCommand(Query, c).ExecuteNonQuery();
		}
		public static SQLiteDataReader ExecReader(string Query)
		{
			return new SQLiteCommand(Query, c).ExecuteReader();
		}
		public static object ExecScalar(string Query)
		{
			return new SQLiteCommand (Query, c).ExecuteScalar ();
		}
		public static void ExecNonQuery(SQLiteCommand Query)
		{
			Query.Connection = c;
			Query.ExecuteNonQuery ();
		}
		public static SQLiteDataReader ExecReader(SQLiteCommand Query)
		{
			Query.Connection = c;
			return Query.ExecuteReader();
		}
		public static object ExecScalar(SQLiteCommand Query)
		{
			Query.Connection = c;
			return Query.ExecuteScalar ();
		}
	}
}
