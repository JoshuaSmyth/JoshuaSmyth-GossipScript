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


        private ulong CombineId(uint scriptId, uint referenceId)
        {
            return (ulong) ((ulong)(scriptId << 32) | (ulong) referenceId); 
        }

        private uint RandomUInt32()
        {
            uint thirtyBits = (uint)random.Next(1 << 30);
            uint twoBits = (uint)random.Next(1 << 2);
            uint fullRange = (thirtyBits << 2) | twoBits;

            return fullRange;
        }

        public void GenerateIdsForScript(List<string> fileNames)
        {

            UInt32 scriptId = RandomUInt32();

            throw new NotImplementedException();

            // TODO Tokenize all files

            // Obtain all ids

            // Create new ids where required

            // Write new files to disk (note: first create copy


        }

        public void GenerateIdsForScript(string fileName)
        {
            GenerateIdsForScript(new List<string>() { fileName });
        }

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

        internal ScriptProgram CompileScript(string startScript)
        {
            return CompileScript(startScript, new List<string>());
        }
    }
}
