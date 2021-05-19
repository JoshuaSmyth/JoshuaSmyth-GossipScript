using System;
using System.IO;

namespace TranspileTest.Nodes
{
    class ShowOptionsNode : Node
    {
        public bool SingleChoiceOnly = false;
        public bool RemoveOnSelect = false;

        // Run Time Values
        public bool ReturnOnSelect = false;     // TODO This needs to be instanced off somewhere

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
            if (ReturnOnSelect)
            {
                // We selected an option which results in a return
            }
            else
            {
                foreach (var n in Children)
                {
                    if (n is OptionNode)
                    {
                        var o = n as OptionNode;


                        // TODO this could be shifted into the compiler at somestage.
                        if (this.RemoveOnSelect == true)
                        {
                            o.RemoveOnSelect = true;
                        }

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
            
            // TODO Flesh this out a little more it's a touch error prone
            if (input=='1')
            {
                SelectChild(pc, 0);
                return NodeRunResult.PushChildN;
            }
            if (input == '2')
            {
                SelectChild(pc, 1);
                return NodeRunResult.PushChildN;
            }
            if (input == '3')
            {
                SelectChild(pc, 2);
                return NodeRunResult.PushChildN;
            }
            if (input == 'X' || input == 'x')
            {
                return NodeRunResult.NextCommand;
            }

            return NodeRunResult.Await;
        }

        private void SelectChild(ProgramCounter pc, int index)
        {
            if (index < Children.Count)
            {
                if (Children[index] is OptionNode)
                {
                    var c = Children[index] as OptionNode;
                    if (c.ReturnOnSelect)
                    {
                        this.ReturnOnSelect = true;
                    }
                }
                pc.ReturnRegisterInt32 = index;
            }
        }

        // On Return (e.g coming back up the call stack)
    }
}
