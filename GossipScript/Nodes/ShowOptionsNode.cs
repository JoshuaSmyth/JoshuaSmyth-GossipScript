using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspileTest.Nodes
{
    class ShowOptionsNode : Node
    {
        public bool SingleChoiceOnly = false;
        public bool RemoveOnSelect = false;
        public ShowOptionsNode()
        {
            NodeType = NodeType.OptionsChoice;
        }

        public override void writeData(BinaryWriter bw)
        {
            bw.Write((UInt16)0);
            bw.Write(SingleChoiceOnly);
            bw.Write(RemoveOnSelect);
        }

        public override void readData(BinaryReader br)
        {
            br.ReadUInt16();
            SingleChoiceOnly = br.ReadBoolean();
            RemoveOnSelect = br.ReadBoolean();
        }

        internal OptionNode AddOption(string v, bool removeOnSelect)
        {
            var rv = new OptionNode(v, removeOnSelect);
            Children.Add(rv);
            return rv;
        }

        public override NodeRunResult Run(ref ProgramCounter pc, ref ProgramData pd)
        {
            var i = 0;
            foreach (var n in Children)
            {
                if (n is OptionNode)
                {
                    var o = n as OptionNode;
                    if (o.RemoveOnSelect && pd.HasRemovedOption(o.Id))
                    {

                    }
                    else
                    {
                        i++;
                        Console.WriteLine(o.Text);
                    }
                }
            }
            // If there are no options return nextnode
            if (i==0)
            {
                return NodeRunResult.NextCommand;
            }
            
            {
                return OnAwaiting(ref pc);
            }
        }

        // On Re-entrant (i.e while awaiting)
        public NodeRunResult OnAwaiting(ref ProgramCounter pc)
        {
            var input = Console.ReadKey().KeyChar;
            Console.WriteLine();

            // TODO Flesh this out a little more
            // TODO Mark as remove on selected if flag set
            if (input=='1')
            {
                pc.ReturnRegisterInt32 = 0;
                return NodeRunResult.PushChildN;
            }
            if (input == '2')
            {
                pc.ReturnRegisterInt32 = 1;
                return NodeRunResult.PushChildN;
            }
            if (input == '3')
            {
                pc.ReturnRegisterInt32 = 2;
                return NodeRunResult.PushChildN;
            }
            if (input == 'X' || input == 'x')
            {
                return NodeRunResult.NextCommand;
            }

            return NodeRunResult.Await;
        }

        // On Return (e.g coming back up the call stack)
    }
}
