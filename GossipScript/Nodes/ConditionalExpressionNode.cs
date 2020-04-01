using ExpressionParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspileTest.ExpressionParser;
using TranspileTest.Parser;

namespace TranspileTest.Nodes
{
    public class ConditionalExpressionNode : Node
    {
        public Expression Expression;

        public ConditionalExpressionNode()
        {
            this.NodeType = NodeType.ConditionalIf;
        }

        public override NodeRunResult Run(ref ProgramCounter pc, ref ProgramData pd)
        {
            Console.WriteLine("TODO Eval Expression");

            var value = true; //pd.ScriptVariableTable.GetValueBool("$bTalked_to_man");

            var context = new ExpressionContext(new HostSymbolTable()); // TODO This should be provided from somewhere.
            context.scriptVariabeTable = pd.ScriptVariableTable; // TODO Should be provided


            var result = pd.expressionRunner.Evaluate(Expression, context);

            if (result == 1)
            {
                return NodeRunResult.PushChildTrue;
            }
            else
            {
                return NodeRunResult.PushChildFalse;
            }
        }

        public override void writeData(BinaryWriter bw)
        {
            bw.Write((UInt16)0);

            // TODO Write expression
            /*
            bw.Write(expression.Instructions.Count);
            foreach(var i in expression.Instructions)
            {
                bw.Write((int)i.TokenType);
                bw.Write(i.Data);
            }*/
        }

        public override void readData(BinaryReader br)
        {
            br.ReadUInt16();
            //expression = br.ReadString();
        }
    }
}
