using ExpressionParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspileTest.ExpressionParser;

namespace TranspileTest.Nodes
{
    public class SetVarNode : Node
    {
        public Expression expression;        // TODO Convert value to expression
        public string variableId;   // TODO Use a guid?

        public SetVarNode()
        {
            NodeType = NodeType.Command;
            CommandType = CommandType.SetVar;
        }

        // TODO The variables need to be added to program data
        public override NodeRunResult Run(ref ProgramCounter pc, ref ProgramData pd)
        {
            
            if (pd.ScriptVariableTable.HasVariable(variableId))
            {
                Console.WriteLine("TODO Provide variable");
                var context = new ExpressionContext(new HostSymbolTable()); // TODO This should be provided from somewhere.
                context.scriptVariabeTable = pd.ScriptVariableTable; // TODO Should be provided
                

                var result = pd.expressionRunner.Evaluate(expression, context);


                // TODO What if the return value of an expression is expected to be a string?
                pd.ScriptVariableTable.SetVariable(variableId, result.ToString());
            }
            else
            {
                // Error invalid variable (Should never be unknown because should be checked at compiletime. 
            }

            return NodeRunResult.NextCommand;
        }

        public override void writeData(BinaryWriter bw)
        {
            bw.Write((UInt16)0); // Version
            //bw.Write(value);
            bw.Write(variableId);
        }

        public override void readData(BinaryReader br)
        {
            br.ReadUInt16();
            //value = br.ReadString();
            variableId = br.ReadString();
        }
    }
}
