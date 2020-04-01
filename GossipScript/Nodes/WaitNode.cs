using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TranspileTest.Nodes
{
    public class WaitNode : Node
    {
        public int WaitTimeMilliseconds;

        private float Counter; // InstanceVariable

        public WaitNode()
        {
            NodeType = NodeType.Command;
            CommandType = CommandType.Wait;
        }

        public override NodeRunResult Run(ref ProgramCounter pc, ref ProgramData pd)
        {
           // Console.WriteLine("Node wait: " + Counter);
            if (Counter < WaitTimeMilliseconds)
            {
                Counter += pd.DeltaTime;
                return NodeRunResult.Await;
            }
            //Console.WriteLine("Node wait done");
            //Thread.Sleep(WaitTimeMilliseconds); // Not good implementation
            return NodeRunResult.NextCommand;
        }

        public override void writeData(BinaryWriter bw)
        {
            bw.Write((UInt16)0);
            bw.Write(WaitTimeMilliseconds);
        }

        public override void readData(BinaryReader br)
        {
            br.ReadUInt16();
            WaitTimeMilliseconds = br.ReadInt32();
        }
    }
}
