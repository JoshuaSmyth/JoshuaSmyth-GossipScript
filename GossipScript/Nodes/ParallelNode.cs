using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspileTest.Nodes
{
    public class ParallelNode : Node
    {

        public ParallelNode()
        {
            NodeType = NodeType.ParallelNode;
        }

        public override NodeRunResult Run(ref ProgramCounter pc, ref ProgramData pd)
        {
            return NodeRunResult.PushParallel;
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
