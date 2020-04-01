using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspileTest.Parser;

namespace TranspileTest.ExpressionParser
{
    public class Expression
    {
        /// <summary>
        /// A sequence of instructions to execute in RPN form
        /// </summary>
        public List<ExpressionToken> Instructions { get; set; }

        // TODO Work out the return type of an expression
    }
}
