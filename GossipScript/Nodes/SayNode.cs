using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspileTest.Nodes
{
    public class SayNode : Node
    {
        public string ActorId;
        public string Text;
        public string Position;

        public SayNode()
        {
            NodeType = NodeType.Command;
            CommandType = CommandType.Say;
            Position = "";
        }

        public SayNode(String actorId, String text, String position)
        {
            ActorId = actorId;
            Text = text;
            Position = position;
            NodeType = NodeType.Command;
            CommandType = CommandType.Say;
        }

        public override NodeRunResult Run(ref ProgramCounter pc, ref ProgramData pd)
        {
            Console.WriteLine(ActorId + ": " + Text);
            return NodeRunResult.NextCommand;
        }

        public override void writeData(BinaryWriter bw)
        {
            bw.Write((UInt16)2);
            bw.Write(ActorId);
            bw.Write(Text);
            bw.Write(Position);
        }

        public override void readData(BinaryReader br)
        {
            br.ReadUInt16();
            ActorId = br.ReadString();
            Text = br.ReadString();
            Position = br.ReadString();
        }
    }
}
