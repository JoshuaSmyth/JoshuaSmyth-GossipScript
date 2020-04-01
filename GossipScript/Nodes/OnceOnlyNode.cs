using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspileTest.Nodes
{
    public class OnceOnlyNode : Node
    {
        public bool RemoveOnSelect;

        public OnceOnlyNode()
        {
            this.NodeType = NodeType.OnceOnly;
        }

        public override NodeRunResult Run(ref ProgramCounter pc, ref ProgramData pd)
        {
            if (pd.HasRunOnceOnly(this.Id))
            {
                return NodeRunResult.PushChildFalse;
            }
            else
            {
                pd.MarkOnceOnly(this.Id);
                return NodeRunResult.PushChildTrue;
            }
        }

        public override void writeData(BinaryWriter bw)
        {
            bw.Write((UInt16)0);
            bw.Write(RemoveOnSelect);
        }

        public virtual void readData(BinaryReader br)
        {
            br.ReadUInt16();
            RemoveOnSelect = br.ReadBoolean();
        }
    }
}

