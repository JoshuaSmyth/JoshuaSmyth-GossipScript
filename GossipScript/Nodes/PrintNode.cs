using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspileTest.Nodes;

namespace TranspileTest.Nodes
{

    public class PrintNode : Node
    {
        public string Text;

        public PrintNode()
        {
            NodeType = NodeType.Command;
            CommandType = CommandType.Print;
        }
        public PrintNode(String text)
        {
            Text = text;
            NodeType = NodeType.Command;
            CommandType = CommandType.Print;
        }

        public override NodeRunResult Run(ref ProgramCounter pc, ref ProgramData pd)
        {
            Console.WriteLine("PRINT:" + Text);
            return NodeRunResult.NextCommand;
        }

        public override void writeData(BinaryWriter bw)
        {
            bw.Write((UInt16)0);
            bw.Write(Text);
        }

        public override void readData(BinaryReader br)
        {
            br.ReadUInt16();
            Text = br.ReadString();
        }
    }
}
