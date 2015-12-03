using System;
using System.Collections.Generic;
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
                    return CDSData.FromRaw(ReadRaw());
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
                    WriteRaw(Data.ToRaw());
                    break;
                case NodeType.Stream:
                    //shitshitshit shitshitshit
                    throw new System.NotImplementedException("building tax!");
            }
        }
        public byte[] ReadRaw() 
        {
            return TableUtils.GetBytes((uint)Id);
        }
        public void WriteRaw(byte[] Data) 
        {
            TableUtils.SetBytes((uint)Id, Data);
        }
        public override Node AddChild(NodeType type, string Name)
        {
            uint NewId = TableUtils.GetLowestAvailableID();
            //creating node:
            NodeData n = new NodeData() { Name = ValidateName(Name), type = type, ParentID = Id };
            TableUtils.WriteToTable(new TableKey() { Table = TableType.Nodes, Node = NewId, Section = 0 }, n);
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
    }
}