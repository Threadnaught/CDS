using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.Common
{
    public abstract class Node
    {
        public abstract string GetName();
        public abstract string GetFullName();
        public abstract NodeType GetType();
        public abstract Node GetParent();
        public abstract List<Node> GetChildren();
        public abstract void Delete();
        public abstract CDSData Read();
        public abstract void Write(CDSData Data);
        public abstract Node AddChild(NodeType type, string Name);
        public abstract bool GetIfExists();

        public Node ChildByName(string Name) 
        {
            return GetChildren().Find(m => m.GetName() == ValidateName(Name));
        }
        public static bool IsValidName(string s)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(s, "^[a-zA-Z0-9\\-_]*$");
        }
        public static string ValidateName(string s)
        {
            return s.ToLower();
        }
        public override string ToString()
        {
            return GetFullName();
        }
    }
}