using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspileTest.Nodes
{
    public class PageNode : Node
    {
        public string PageName;

        public PageNode()
        {
            NodeType = NodeType.Page;
        }

        public PageNode(String name)
        {
            PageName = name;
            NodeType = NodeType.Page;
        }

        public override void writeData(BinaryWriter bw)
        {
            bw.Write((UInt16)0);
            bw.Write(PageName);
        }

        public override void readData(BinaryReader br)
        {
            br.ReadUInt16();
            PageName = br.ReadString();
        }
    }
}
