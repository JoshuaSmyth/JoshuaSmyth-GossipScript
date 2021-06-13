using NUnit.Framework;
using TranspileTest;
using System.IO;
using TranspileTest.Parser;

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
            //11189196
            var scriptString = ReadFile("Scripts/test001.gs");
            var scriptProgram = new ScriptProgram();

            var parser = new ScriptParser();
            var script1 = parser.ParseScript(scriptProgram, scriptString);

            // Check the Gossip Script Id is correct
            Assert.IsTrue(script1.Id == 11189196);

            // TODO Check the page Ids are correct.
            //Assert.IsTrue(script1.Id == 11189196);

        }

        [Test]
        public void TestIdentifierAndPrettifier()
        {
            var scriptParser = new ScriptParser();

            // TODO Write StringifyString() method
            // TODO Write StringifyFile() method

            var tokenStream = scriptParser.TokenizeString(ReadFile("Scripts/test002.gs"), discardWhitespace:true, discardComments:false);

            // Reconstruct the text from the tokens
            var newTokenStreamSet = scriptParser.Identify(new TokenStreamSet(tokenStream));

            TestContext.Out.WriteLine();
            TestContext.Out.WriteLine("New Stream");
            TestContext.Out.WriteLine();

            foreach (var stream in newTokenStreamSet.TokenStreams)
            {
                var output = stream.Stringify();
                TestContext.Out.WriteLine(output);
            }
        }
    }
}
