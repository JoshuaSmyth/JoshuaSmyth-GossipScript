using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspileTest.Nodes
{
    public class OptionNode : Node
    {
        public string Text;
        public bool RemoveOnSelect;
        public bool ExitOnSelect;

        public OptionNode()
        {
            NodeType = NodeType.Option;
        }

        public override void writeData(BinaryWriter bw)
        {
            bw.Write((UInt16)0);
            bw.Write(Text);
            bw.Write(RemoveOnSelect);
            bw.Write(ExitOnSelect);
        }

        public override void readData(BinaryReader br)
        {
            br.ReadUInt16();
            Text = br.ReadString();
            RemoveOnSelect = br.ReadBoolean();
            ExitOnSelect = br.ReadBoolean();
        }
        public OptionNode(string text, bool removeOnSelection = false)
        {
            Text = text;
            NodeType = NodeType.Option;
            RemoveOnSelect = removeOnSelection;
        }

        public override NodeRunResult Run(ref ProgramCounter pc, ref ProgramData pd)
        {
            if (RemoveOnSelect)
            {
                pd.RemoveOption(this.Id);
            }
            
            pc.ReturnRegisterInt32 = 0;
            return NodeRunResult.PushChildN;
        }
    }
}
