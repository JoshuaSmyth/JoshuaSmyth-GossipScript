using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using TranspileTest;
using TranspileTest.ExpressionParser;
using TranspileTest.Parser;

namespace ExpressionParser
{
    public class ExpressionRunner
    {
        readonly ExpressionCompiler m_Compiler;

        readonly HostCallTable m_CallTable = new HostCallTable();

        public ExpressionRunner()
        {
            m_Compiler = new ExpressionCompiler(m_CallTable);

            // Add some built in functions
            RegisterFunction("cos", null, new Func<double, double>(Math.Cos));
            RegisterFunction("sin", null, new Func<double, double>(Math.Sin));
            RegisterFunction("exp", null, new Func<double, double>(Math.Exp));
            RegisterFunction("max", null, new Func<double, double, double>(Math.Max));
        }

        public void RegisterFunction(string name, object owner, Delegate function)
        {
            Console.WriteLine("TODO Implement custom functions");
           // m_CallTable.RegisterFunction(name, function);
          //  m_Compiler.Tokenizer.AddToken(new SemanticToken(new Regex(name), TokenType.FunctionCall, OperationType.FunctionCall));
        }

        public double Evaluate(Expression expression, ExpressionContext context)
        {
            var opcodes = expression.Instructions;
            var stack = context.EvaluationStack;
            var symbolTable = context.SymbolTable;

            foreach (var opcode in opcodes)
            {
                if (opcode.IsValue())
                {
                    // TODO If this is a variable it needs to read the variable name (later guid)
                    // And convert it to the correct number.
                    if (opcode.IsVariableNumberType())
                    {
                        var value = context.scriptVariabeTable.GetValueInt(opcode.Data.Guid);


                        //Console.WriteLine("TODO Look up variable");
                        
                        // TODO Lookup the variable
                        stack.Push(value);
                    }
                    else
                    {
                        stack.Push(opcode.Data.Float);
                    }
                }
                else
                {
                    // Evaluate opcode
                    switch (opcode.TokenType)
                    {
                        case SemanticTokenType.Add:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                stack.Push(lhs + rhs);
                                break;
                            }
                        case SemanticTokenType.Subtract:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                stack.Push(lhs - rhs);
                                break;
                            }
                        case SemanticTokenType.Multiply:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                stack.Push(lhs * rhs);
                                break;
                            }
                        case SemanticTokenType.Divide:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                stack.Push(lhs / rhs);
                                break;
                            }
                        case SemanticTokenType.PowerOf:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                stack.Push(Math.Pow(lhs, rhs));
                                break;
                            }
                        case SemanticTokenType.UnaryMinus:
                            {
                                var rhs = stack.Pop();
                                stack.Push(rhs * -1);
                                break;
                            }
                        case SemanticTokenType.Modulo:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                stack.Push(lhs % rhs);
                                break;
                            }
                        case SemanticTokenType.Negation:
                            {
                                var rhs = stack.Pop();
                                stack.Push((rhs == 0) ? 1 : 0);
                                break;
                            }
                        case SemanticTokenType.GreaterThan:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                var data = (lhs > rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case SemanticTokenType.LessThan:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                var data = (lhs < rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case SemanticTokenType.GreaterThanOrEqualTo:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                var data = (lhs >= rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case SemanticTokenType.LessThanOrEqualTo:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                var data = (lhs <= rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case SemanticTokenType.Equal:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                var data = (lhs == rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case SemanticTokenType.NotEqual:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                var data = (lhs != rhs) ? 1 : 0;
                                stack.Push(data);
                                break;
                            }
                        case SemanticTokenType.LogicalAnd:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                var d1 = (rhs == 0) ? false : true;
                                var d2 = (lhs == 0) ? false : true;
                                var d3 = d1 && d2;
                                var data = (d3 == true) ? 1.0d : 0.0d;
                                stack.Push(data);
                                break;
                            }
                        case SemanticTokenType.LogicalOr:
                            {
                                var rhs = stack.Pop(); var lhs = stack.Pop();
                                var d1 = (rhs == 0) ? false : true;
                                var d2 = (lhs == 0) ? false : true;
                                var d3 = d1 || d2;
                                var data = (d3 == true) ? 1.0d : 0.0d;
                                stack.Push(data);
                                break;
                            }
                        case SemanticTokenType.FunctionCall:
                            {
                                // TODO Might need a string table
                                var functionId = (Int32)opcode.Data.Int;
                                var function = m_CallTable.GetFunctionById(functionId);
                                var parameters = function.ParameterList;
                                for (int i = 0; i < parameters.Length; i++)
                                {
                                    parameters[i] = stack.Pop();
                                }
                                var data = function.Invoke();
                                stack.Push(data);
                                break;
                            }
                        case SemanticTokenType.Symbol:
                            {
                                // This is probably a variable lookup

                                var symbolId = (Int32)opcode.Data.Int;
                                var symbol = symbolTable.GetSymbolById(symbolId);
                                stack.Push(symbol.SymbolValue);
                                break;
                            }
                        default:
                            throw new Exception(String.Format("Unknown operator{0}", opcode));
                            break;
                    }
                }
            }

            // If more than one value on the stack error with input
            var r = stack.Pop();
            return r;
        }

        public void RegisterSymbol(String symbolName)
        {
            throw new NotImplementedException("TODO");

            //m_Compiler.Tokenizer.RegisterSymbol(symbolName);
            //m_Compiler.SemanticAnalyser.HostSymbolTable.RegisterSymbol(symbolName, 0);

        }

        public void ClearAllSymbols()
        {
            throw new NotImplementedException("TODO");

            // m_Compiler.Tokenizer.ClearAllSymbols();
        }
    }
}
