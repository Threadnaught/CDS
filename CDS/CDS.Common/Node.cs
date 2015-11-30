using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.Common
{
    public abstract class Node
    {
        public abstract string Name();
        public abstract string FullName();
        public abstract NodeType Type();
        public abstract Node Parent();
        public abstract List<Node> Children();
        public abstract void Delete();
        public abstract CDSData Read();
        public abstract void Write(CDSData Data);
        public abstract Node AddChild(NodeType type, string Name);

        public static bool ValidName(string s)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(s, "^[a-zA-Z0-9\\-_]*$");
        }
        public static string ValidateName(string s)
        {
            return s.ToLower();
        }
        public override string ToString()
        {
            return FullName();
        }
    }
}
