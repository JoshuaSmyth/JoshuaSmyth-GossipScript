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

        public Dictionary<Guid, ScriptNode> Scripts = new Dictionary<Guid, ScriptNode>();

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
