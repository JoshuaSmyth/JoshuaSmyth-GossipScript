using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TranspileTest;
using TranspileTest.Parser;

namespace ExpressionParser
{
    public class ExpressionContext
    {
        /// <summary>
        /// The symbols to be resolved at execution time
        /// </summary>
        public HostSymbolTable SymbolTable { get; set; }    // TODO Might remove these and add a variable lookup operator instead

        /// <summary>
        /// The stack used by the evaluation process
        /// </summary>
        public Stack<Double> EvaluationStack { get; set; }


        public ScriptVariableTable scriptVariabeTable { get; set; }

        public ExpressionContext(HostSymbolTable symbolTable)
        {
            SymbolTable = symbolTable;
            EvaluationStack = new Stack<Double>();
            //Instructions = new List<ExpressionToken>();
        }

        public void SetSymbol(string s, int i)
        {
           SymbolTable.RegisterSymbol(s,i);
        }
    }
}
