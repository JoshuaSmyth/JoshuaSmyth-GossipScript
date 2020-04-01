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
        public static void AssignRandomGuids(Node root)
        {
            root.Id = Guid.NewGuid();
            foreach(var r in root.Children)
            {
                AssignRandomGuids(r);
            }
        }
    }
}
