using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Data;
using System.Linq;

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
                return TableUtils.GetNodeData((uint)Id).ParentID;
            }
        }
        public override string Name()
        {
            return TableUtils.GetNodeData((uint)Id).Name;
            //set:
                //SQLiteCommand c = new SQLiteCommand("UPDATE Nodes SET Name=@Contents WHERE Id =" + Id.ToString());
                //c.Parameters.Add("@Contents", DbType.String).Value = ValidateName(value);
                //SqliteWrapper.ExecNonQuery(c);
        }
        public override string FullName()
        {
            if (ParentId != -1)
            {
                return Parent().FullName() + "." + Name();
            }
            else
            {
                return Name();
            }
        }
        public override NodeType Type()
        {
            return TableUtils.GetNodeData((uint)Id).type;
        }
        public override Node Parent()
        {
            return new LocalNode() { Id = ParentId };
        }
        public override List<Node> Children()
        {
            List<Node> Ret = new List<Node>();
            UInt32[] IDs = TableUtils.GetChildren((uint)Id);
            foreach (UInt32 id in IDs) 
            {
                Ret.Add(new LocalNode() { Id = (int)id });
            }
            return Ret;
        }
        public override void Delete()
        {
            int Parent = TableUtils.GetNodeData((uint)Id).ParentID;
            if (Parent != -1)
            {
                List<UInt32> Children = TableUtils.GetChildren((uint)Parent).ToList();
                Children.Remove((uint)Id);
                TableUtils.SetChildren((uint)Parent, Children.ToArray());
            }
            TableUtils.Remove((uint)Id);
            Id = -1;
        }
        public override CDSData Read()
        {
            switch (Type())
            {
                case NodeType.Hollow:
                    return null;
                case NodeType.Data:
                    return CDSData.FromRaw(TableUtils.GetBytes((uint)Id));
                case NodeType.Stream:
                    //shitshitshit
                    throw new System.NotImplementedException("go directly to prison, do not pass go, do not collect £200");
                default:
                    throw new System.NotImplementedException("you have won second prize in a beauty contest. collect £20");
            }
        }
        public override void Write(CDSData Data)
        {
            switch (Type())
            {
                case NodeType.Hollow:
                    break;
                case NodeType.Data:
                    TableUtils.SetBytes((uint)Id, Data.ToRaw());
                    break;
                case NodeType.Stream:
                    //shitshitshit shitshitshit
                    throw new System.NotImplementedException("building tax!");
            }
        }
        public override Node AddChild(NodeType type, string Name)
        {
            uint NewId = TableUtils.GetLowestAvailableID();
            //creating node:
            NodeData n = new NodeData() { Name = Name, type = type, ParentID = Id };
            TableUtils.WriteToTable(new Key() { Table = TableType.Nodes, Node = NewId, Section = 0 }, n);
            //adding to child list:
            List<UInt32> Children = TableUtils.GetChildren((uint)Id).ToList();
            Children.Add(NewId);
            TableUtils.SetChildren((uint)Id, Children.ToArray());
            return new LocalNode() { Id = (int)NewId };
        }

        public static LocalNode Root
        {
            get
            {
                return new LocalNode()
                {
                    Id = 0
                };
            }
        }
        public static LocalNode ByName(string Name)
        {
            SQLiteCommand c = new SQLiteCommand("SELECT Id FROM Nodes WHERE Name = @Name");
            c.Parameters.Add("@Name", DbType.String).Value = ValidateName(Name);
            return new LocalNode() { Id = Convert.ToInt32(SqliteWrapper.ExecScalar(c)) };
        }
        public static LocalNode Resolve(string Name)
        {
            string[] sections = Name.Split('.');
            LocalNode n = Root;
            foreach (string s in sections)
            {
                if (ValidateName(s) == "root" && n == Root) continue;
                if (s != "")
                    foreach (LocalNode c in n.Children())
                    {
                        if (ValidateName(s) == c.Name())
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
            return new LocalNode() { Id = Create(Parent.Id, Name, Type, Contents) };
        }
        public static int Create(int ParentId, string Name, NodeType Type, byte[] Contents)
        {
            if (!ValidName(Name)) throw new ArgumentException("Invalid chars in name");
            SQLiteCommand c = new SQLiteCommand("INSERT INTO Nodes (ParentId, Name, Type, Contents) VALUES (@ParentId, @Name, @Type, @Contents)");
            c.Parameters.Add("@ParentId", DbType.Int32).Value = ParentId;
            c.Parameters.Add("@Name", DbType.String).Value = ValidateName(Name);
            c.Parameters.Add("@Type", DbType.Int32).Value = (int)Type;
            c.Parameters.Add("@Contents", DbType.Binary).Value = Contents;

            SqliteWrapper.ExecNonQuery(c);
            return SqliteWrapper.LastInsertRowId;
        }
    }
}