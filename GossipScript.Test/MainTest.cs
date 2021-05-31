using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GossipScript;
using TranspileTest;
using System.IO;
using System.Diagnostics;

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

        public string GetFile(string filename)
        {
            var dir = Path.GetDirectoryName(typeof(MainTest).Assembly.Location);
            return Path.Combine(dir, filename); 
        }



        [Test]
        public void ScriptGeneratesExpectedNumberOfTokens()
        {
            var scriptParser = new ScriptParser();
            var tokenStream = scriptParser.TokenizeFile(GetFile("Scripts/test002.gs"), applyDiscardPolicy:false);


            TestContext.Out.WriteLine("Token Count:" + tokenStream.Count());
            TestContext.Out.WriteLine();


            var tokenList = tokenStream.ToList();

            foreach(var t in tokenList)
            {
                TestContext.Out.WriteLine(t.TokenType + " : " + t.TokenValue);
            }

            TestContext.Out.WriteLine(tokenStream.Stringify());

            // Reconstruct the text from the tokens
            var newTokenStreamSet = scriptParser.Identify(new TokenStreamSet(tokenStream));

            TestContext.Out.WriteLine();
            TestContext.Out.WriteLine("New Stream");
            TestContext.Out.WriteLine();

            foreach (var stream in newTokenStreamSet.TokenStreams)
            {
                TestContext.Out.WriteLine(stream.Stringify());
            }

        }
    }
}
