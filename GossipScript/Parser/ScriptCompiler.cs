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

        Random random = new Random(); // TODO Random service

        private uint RandomUInt32()
        {
            uint thirtyBits = (uint)random.Next(1 << 30);
            uint twoBits = (uint)random.Next(1 << 2);
            uint fullRange = (thirtyBits << 2) | twoBits;

            return fullRange;
        }

        public ScriptProgram CompileProgram(List<string> scripts)
        {
            var rv = new ScriptProgram();
           
            foreach (var f in scripts)
            {
                var node = scriptParser.ParseScript(rv, f);
                if (rv.Scripts.Count() == 0)
                {
                    rv.AddScript(node);
                }
                else
                {
                    rv.AddMainScript(node);
                }
            }


            return rv;
        }

        public ScriptProgram CompileProgram(string startFilename, List<string> fileNames)
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



            // FIXME = We shouldn't need to do this.
            TestScripts.AssignRandomIdentifiers(rv);
            // ENDFIX ME

            return rv;
        }

        internal ScriptProgram CompileScript(string startScript)
        {
            return CompileProgram(startScript, new List<string>());
        }
    }
}
