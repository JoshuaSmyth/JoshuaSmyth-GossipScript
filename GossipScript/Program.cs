using System;
using System.Collections.Generic;
using System.IO;
using TranspileTest.Nodes;
using TranspileTest.Parser;

namespace TranspileTest
{
    class Program
    {
        static NodeEngine nodeEngine = new NodeEngine();

        static ScriptParser ScriptParser = new ScriptParser();


        static void Main(string[] args)
        {

            var scriptCompiler = new ScriptCompiler();

            var program_a = scriptCompiler.CompileScript("Scripts/test002.gs");
            nodeEngine.RunScriptContinous(program_a.MainScript);
        }

        private static ScriptNode ParseScript(string input)
        {
            using (new ScoppedTimer("ParseScript"))
            {
                var scriptProgram = new ScriptProgram();
                var script1 = ScriptParser.ParseScript(scriptProgram, input);
                return script1;
            }
        }

        private static ScriptNode ParseBinaryScript(byte[] input)
        {
            using (new ScoppedTimer("ReadBinaryScript"))
            {
                var script1 = Serializer.Read(input);
                return script1;
            }
        }



        private static ScriptNode LoadScript(string filename)
        {
            var script4 = ParseScript(File.ReadAllText("Scripts/" + filename));
            TestScripts.AssignRandomIdentifiers(script4);
            var serial4 = Serializer.Write(script4);
            script4 = ParseBinaryScript(serial4);

            File.WriteAllBytes(filename + ".b.script", serial4);
            Console.WriteLine("Byte Size:" + serial4.Length);
            return script4;
        }

        private static void PrintNodeTree(Node node, int level)
        {
            if (node.NodeType == NodeType.Command)
            {
                Console.WriteLine(node.NodeType + ":" + node.CommandType);
            }
            else
            {
                Console.WriteLine(node.NodeType);
            }

            foreach(var c in node.Children)
            {
                for (int i = 0; i < level; i++)
                {
                    Console.Write(" ");
                }
                
                PrintNodeTree(c, ++level);
            }
        }
    }
}
