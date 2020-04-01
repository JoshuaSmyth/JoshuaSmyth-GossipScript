using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspileTest.Nodes;

namespace TranspileTest.Parser
{
    public class ScriptCompiler
    {
        ScriptParser scriptParser = new ScriptParser();

        public ScriptProgram CompileScript(string startFilename, List<string> fileNames)
        {
            var rv = new ScriptProgram();
            rv.AddMainScript(LoadAndParseFile(rv, startFilename));

            foreach(var f in fileNames)
            {
                var node = LoadAndParseFile(rv, f);
                rv.AddScript(node);
            }
            return rv;
        }


        ScriptNode LoadAndParseFile(ScriptProgram scriptProgram, string filename)
        {
            var text = File.ReadAllText(filename);
            var rv = scriptParser.ParseScript(scriptProgram, text);

            // TODO Fix up this mess!
            TestScripts.AssignRandomGuids(rv);

            return rv;
        }

    }
}
