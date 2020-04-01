using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspileTest.Nodes
{
    public class ReturnNode : Node
    {
        public ReturnNode()
        {
            NodeType = NodeType.Command;
            CommandType = CommandType.Return;
        }

        public override NodeRunResult Run(ref ProgramCounter pc, ref ProgramData pd)
        {
            return NodeRunResult.PopPage;
        }

        public override void writeData(BinaryWriter bw)
        {
            bw.Write((UInt16)0);
        }

        public override void readData(BinaryReader br)
        {
            br.ReadUInt16();
        }
    }
}
