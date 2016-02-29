using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Linq;

namespace CDS.Common
{
    public class Node1
    {
        public bool Local;
        public string FullName;
        public string GetName()
        {
            return FullName.Split('.').Last();
        }
        public string GetFullName()
        {
            return FullName;
        }
        public NodeType GetNodeType()
        {
            if (Local)
            {
                return TableUtils.GetNodeData((uint)lookupID(FullName)).type;
            }
            else
            {
                //remote stuff
            }
        }
        public Node1 GetParent()
        {
            if (Local)
            {
                return new Node1() { Local = true, FullName = TableUtils.GetNodeData((uint)TableUtils.GetNodeData((uint)lookupID(FullName)).ParentID).Name };
            }
            else
            {
                //remote stuff
            }
        }
        string FullNameFromId(uint id)
        {
            if ((uint)TableUtils.GetNodeData((uint)lookupID(FullName)).ParentID == 0) { return TableUtils.GetNodeData(id).Name; }
            return FullNameFromId((uint)TableUtils.GetNodeData((uint)lookupID(FullName)).ParentID) + "." + TableUtils.GetNodeData((uint)lookupID(FullName)).Name;
        }
        uint lookupID(string Name)
        {
            if (!Local) throw new Exception("wrong side, try the other one");
            bool Advanced = false;
            uint CurrentID = 0;//start at root
            foreach (string s in Name.Split('.'))
            {
                foreach (uint possible in TableUtils.GetChildren(CurrentID))
                    if (TableUtils.GetNodeData(possible).Name == Node.ValidateName(s))
                    {
                        CurrentID = possible;
                        Advanced = true;
                        break;
                    }
                if (!Advanced) throw new Exception("node you were looking for doesn't exist");
                Advanced = false;
            }
            return CurrentID;
        }
    }
    public class Node
    {
        int Id;
        int ParentId
        {
            get
            {
                return TableUtils.GetNodeData((uint)Id).ParentID;
            }
        }
        public virtual string GetName()
        {
            return TableUtils.GetNodeData((uint)Id).Name;
        }
        public virtual string GetFullName()
        {
            if (ParentId != -1)
            {
                return GetParent().GetFullName() + "." + GetName();
            }
            else
            {
                return GetName();
            }
        }
        public virtual NodeType GetNodeType()
        {
            return TableUtils.GetNodeData((uint)Id).type;
        }
        public virtual Node GetParent()
        {
            return new Node() { Id = ParentId };
        }
        public virtual List<Node> GetChildren()
        {
            List<Node> Ret = new List<Node>();
            UInt32[] IDs = TableUtils.GetChildren((uint)Id);
            foreach (UInt32 id in IDs) 
            {
                Ret.Add(new Node() { Id = (int)id });
            }
            return Ret;
        }
        public virtual void Delete()
        {
            foreach (Node n in GetChildren()) n.Delete();

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
        public virtual CDSData Read()
        {
            switch (GetNodeType())
            {
                case NodeType.Hollow:
                    return null;
                case NodeType.Data:
                    return CDSData.FromRaw(ReadRaw());
                case NodeType.Stream:
                    return CDSCode.FromRaw(ReadRaw()).Read();
                default:
                    throw new System.NotImplementedException("you have won second prize in a beauty contest. collect £20");
            }
        }
        public virtual void Write(CDSData Data)
        {
            switch (GetNodeType())
            {
                case NodeType.Hollow:
                    break;
                case NodeType.Data:
                    WriteRaw(Data.ToRaw());
                    break;
                case NodeType.Stream:
                    //shitshitshit shitshitshit
                    CDSCode.FromRaw(ReadRaw()).Write(Data);
                    break;
            }
        }
        public virtual byte[] ReadRaw() 
        {
            return TableUtils.GetBytes((uint)Id);
        }
        public void WriteRaw(byte[] Data) 
        {
            TableUtils.SetBytes((uint)Id, Data);
        }
        public virtual Node AddChild(NodeType type, string Name)
        {
            uint NewId = TableUtils.GetLowestAvailableID();
            //creating node:
            NodeData n = new NodeData() { Name = ValidateName(Name), type = type, ParentID = Id };
            TableUtils.WriteToTable(new TableKey() { Table = TableType.Nodes, Node = NewId, Section = 0 }, n);
            //adding to child list:
            List<UInt32> Children = TableUtils.GetChildren((uint)Id).ToList();
            Children.Add(NewId);
            TableUtils.SetChildren((uint)Id, Children.ToArray());
            return new Node() { Id = (int)NewId };
        }
        public virtual bool GetIfExists()
        {
            return true; //MIGHT NEED TO CHANGE
        }
        public static Node Root
        {
            get
            {
                return new Node()
                {
                    Id = 0
                };
            }
        }
        public static Node Resolve(string Name)
        {
            string[] sections = Name.Split('.');
            Node n = Root;
            foreach (string s in sections)
            {
                if (ValidateName(s) == "root" && n == Root) continue;
                bool Finished = false;
                if (s != "")
                    foreach (Node c in n.GetChildren())
                    {
                        if (ValidateName(s) == c.GetName())
                        {
                            Finished = true;
                            n = c;
                            break;
                        }
                    }
                if (!Finished && s != "") 
                {
                    return null;
                }
            }
            return n;
        }
        public static string ValidateName(string s)
        {
            return s.ToLower();
        }
    }
}