using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspileTest.Nodes
{
    public class CallPageNode : Node
    {
        public string TargetPage;

        public CallPageNode()
        {
            init();
        }

        private void init()
        {
            NodeType = NodeType.Command;
            CommandType = CommandType.CallPage;
        }

        public CallPageNode(string targetPage)
        {
            this.TargetPage = targetPage;
            init();
        }

        public override NodeRunResult Run(ref ProgramCounter pc, ref ProgramData pd)
        {
            pc.ReturnRegisterString = this.TargetPage;
            return NodeRunResult.PushPage;
        }

        public override void writeData(BinaryWriter bw)
        {
            bw.Write((UInt16)0);
            bw.Write(TargetPage);
        }

        public override void readData(BinaryReader br)
        {
            br.ReadUInt16();
            TargetPage = br.ReadString();
        }
    }
}
