using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspileTest.Nodes;

namespace TranspileTest
{
    public static class TestScripts
    {
        private static Random random = new Random();

        public static void AssignRandomGuids(Node root)
        {
            root.Id = (UInt32)random.Next();

            foreach(var r in root.Children)
            {
                AssignRandomGuids(r);
            }
        }
    }
}
