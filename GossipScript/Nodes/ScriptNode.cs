using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspileTest.Nodes
{
    public class ScriptNode : Node
    {
        public ScriptVariableTable LocalVariables = new ScriptVariableTable();

        public ScriptNode()
        {
            NodeType = NodeType.Script;
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
