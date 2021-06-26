using NUnit.Framework;
using TranspileTest;
using System.IO;
using TranspileTest.Parser;
using System.Collections.Generic;

namespace GossipScript.Test
{
    [TestFixture]
    public class MainTest
    {
        [Test]
        public void TestPass()
        {
            Assert.Pass();
        }

        public string GetFileName(string filename)
        {
            var dir = Path.GetDirectoryName(typeof(MainTest).Assembly.Location);
            return Path.Combine(dir, filename); 
        }

        public string ReadFile(string filename)
        {
            var dir = Path.GetDirectoryName(typeof(MainTest).Assembly.Location);
            var fullName = Path.Combine(dir, filename);

            return File.ReadAllText(fullName);
        }

        [Test]
        public void CompilerCanParseIdentifiersCorrectly()
        {
            // Id should be 11189196
            var scriptString = ReadFile("Scripts/test001.gs");
            var scriptProgram = new ScriptProgram();

            var parser = new ScriptParser();
            var script1 = parser.ParseScript(scriptProgram, scriptString);

            // Check the Gossip Script Id is correct
            Assert.IsTrue(script1.Id == 11189196);
        }

        [Test]
        public void TestIdentifierAndPrettifier()
        {
            var compiler = new ScriptCompiler();
            var scriptParser = new ScriptParser();
    
            var tokenStream = scriptParser.TokenizeString(ReadFile("Scripts/test002.gs"), discardWhitespace:true, discardComments:false);
            var newTokenStreamSet = scriptParser.AssignIdentifiers(new TokenStreamSet(tokenStream));

            TestContext.Out.WriteLine();
            TestContext.Out.WriteLine("New Stream");
            TestContext.Out.WriteLine();

            var scripts = new List<string>();
            foreach (var stream in newTokenStreamSet.TokenStreams)
            {
                var output = stream.Stringify();
                scripts.Add(output);

                TestContext.Out.WriteLine(output);
            }

            var program = compiler.CompileProgram(scripts);

            // TODO Some basic sanity checking

        }
    }
}
