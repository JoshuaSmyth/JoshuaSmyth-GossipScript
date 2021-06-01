using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspileTest.Nodes;

namespace TranspileTest.Parser
{
    public class ScriptProgram
    {
        public ScriptVariableTable GlobalVariables = new ScriptVariableTable();

        public Dictionary<UInt32, ScriptNode> Scripts = new Dictionary<UInt32, ScriptNode>();

        public ScriptNode MainScript;

        public void AddMainScript(ScriptNode node)
        {
            MainScript = node;
            Scripts.Add(node.Id, node);
        }

        public void AddScript(ScriptNode node)
        {
            Scripts.Add(node.Id, node);
        }
    }
}
