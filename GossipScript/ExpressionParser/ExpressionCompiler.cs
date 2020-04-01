using System;
using System.Collections.Generic;
using TranspileTest;
using TranspileTest.Parser;

namespace ExpressionParser
{
    public class ExpressionCompiler
    {
        private readonly HostCallTable m_FunctionTable;

        private readonly SemanticAnalyser m_SemanticAnalyser;

        public ExpressionCompiler(HostCallTable functionTable)
        {
            m_FunctionTable = functionTable;
            // m_HostSymbolTable = hostSymbolTable;
            m_SemanticAnalyser = new SemanticAnalyser(m_FunctionTable);
        }
        
        public SemanticAnalyser SemanticAnalyser
        {
            get { return m_SemanticAnalyser; }
        }
        
        public List<ExpressionToken> ConvertToReversePolishNotation(ScriptVariableTable variableTable, List<InputToken> tokenstream)
        {
            // TODO Probably returns an expression object
            // That contains a list<semantictokens> but also a list of the variable lookups referenced in the list.


            var rv = new List<ExpressionToken>();
            var opcodes = m_SemanticAnalyser.ApplySemantics(variableTable, tokenstream);
            var stack = new Stack<ExpressionToken>(tokenstream.Count);

            // Shunting Yard Algorithm
            // http://en.wikipedia.org/wiki/Shunting-yard_algorithm
            foreach (var opcode in opcodes)
            {
                if (opcode.IsNumber() || opcode.IsVariableNumberType())
                {
                    rv.Add(opcode);
                }
                else
                {
                    if (opcode.IsFunction())
                    {
                        stack.Push(opcode);
                    }
                    else
                    {
                        if (opcode.IsFunctionArgumentSeperator())
                        {
                            while (stack.Count > 0)
                            {
                                var o = stack.Peek();
                                if (o.TokenType == SemanticTokenType.OpenBracket)
                                    break;

                                rv.Add(o);
                            }
                        }
                        else
                        {
                            if (opcode.IsOperator())
                            {
                                while (stack.Count > 0)
                                {
                                    var peek = stack.Peek();
                                    if ((opcode.Precedence < peek.Precedence) ||
                                        opcode.OperatorAssociativity == OperatorAssociativity.Left && opcode.Precedence == peek.Precedence)
                                    {
                                        rv.Add(stack.Pop());
                                    }
                                    else { break; }
                                }

                                stack.Push(opcode);
                            }
                            else
                            {
                                // Left Bracket
                                if (opcode.TokenType == SemanticTokenType.OpenBracket)
                                    stack.Push(opcode);

                                if (opcode.TokenType == SemanticTokenType.CloseBracket)
                                {
                                    var token = stack.Pop();
                                    while (stack.Count > 0 && token.TokenType != SemanticTokenType.OpenBracket)
                                    {
                                        rv.Add(token);
                                        token = stack.Pop();
                                    }

                                    if (token.TokenType != SemanticTokenType.OpenBracket)
                                        throw new ExpressionParserException("Mismatched brackets");
                                }
                            }
                        }
                    }
                }
            }

            // When there are no more tokens to read
            while (stack.Count > 0)
            {
                var op = stack.Pop();
                if (op.IsBracket())
                    throw new ExpressionParserException("Mismatched brackets");

                rv.Add(op);
            }

            return rv;
        }
    }
}
