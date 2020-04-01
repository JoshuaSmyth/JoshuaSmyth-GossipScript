using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TranspileTest;
using TranspileTest.Parser;

namespace ExpressionParser
{
    public class SemanticAnalyser
    {
        private readonly HostCallTable m_HostCallTable;
        private readonly HostSymbolTable m_HostSymbolTable;

        public SemanticAnalyser(HostCallTable hostCallTable)
        {
            m_HostCallTable = hostCallTable;
            m_HostSymbolTable = new HostSymbolTable();
        }

        public HostSymbolTable HostSymbolTable
        {
            get { return m_HostSymbolTable; }
        }

        public IEnumerable<ExpressionToken> ApplySemantics(ScriptVariableTable variableTable, IEnumerable<InputToken> tokenStream)
        {
            var tokens = tokenStream.ToList();
            PreprocessSemantics(ref tokens);
            
            var rv = new List<ExpressionToken>(tokens.Count());
            foreach (var token in tokens)
            {
                if (token.OperationType == TranspileTest.OperationType.Operator)
                {
                    var t = OperatorToOpcode(token);
                    rv.Add(t);
                }
                else
                {
                    switch (token.TokenType)
                    {
                        // TODO Add variable lookups for this we might want some kind of index?
                        // To be stored with the expression?

                        case SemanticTokenType.FunctionCall:
                            {
                                var t = new ExpressionToken
                                {
                                    TokenType = token.TokenType,
                                    OperationType = OperationType.FunctionCall,
                                    Precedence = 9
                                };

                                var function = m_HostCallTable.GetFunctionByName(token.TokenValue);
                                t.Data.Int = function.Id;

                                rv.Add(t);
                                break;
                            }
                        case SemanticTokenType.FunctionArgumentSeperator:
                            {
                                rv.Add(new ExpressionToken { TokenType = token.TokenType, OperationType = OperationType.Operand });
                                break;
                            }
                        case SemanticTokenType.Symbol:
                            {
                                var symbol = HostSymbolTable.GetSymbolByName(token.TokenValue);
                                var expressionData = new ExpressionData() { Int = symbol.SymbolId };
                                rv.Add(new ExpressionToken { TokenType = token.TokenType, OperationType = OperationType.Operator, Data = expressionData, Precedence = 9});
                                break;
                            }
                        default:
                            {
                                var t = new ExpressionToken
                                {
                                    TokenType = token.TokenType,
                                    OperationType = OperationType.Operand
                                };

                                if (!t.IsBracket())
                                {
                                    if (t.TokenType == SemanticTokenType.VariableName)
                                    {
                                        t.Data.Guid = variableTable.GetGuid(token.TokenValue);
                                    }
                                    else
                                    {
                                        t.Data.Float = float.Parse(token.TokenValue);
                                    }
                                }

                                rv.Add(t);
                                break;
                            }
                    }
                }
            }
            return rv;
        }

        private static ExpressionToken OperatorToOpcode(InputToken token)
        {
            var t = new ExpressionToken() { TokenType = token.TokenType, OperationType = token.OperationType };

            switch (t.TokenType)
            {
                case SemanticTokenType.Add:
                    t.Precedence = 6;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case SemanticTokenType.Subtract:
                    t.Precedence = 6;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case SemanticTokenType.Multiply:
                    t.Precedence = 7;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case SemanticTokenType.Divide:
                    t.Precedence = 7;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case SemanticTokenType.PowerOf:
                    t.Precedence = 7;
                    t.OperatorAssociativity = OperatorAssociativity.Right;
                    break;
                case SemanticTokenType.UnaryMinus:
                    t.Precedence = 8;
                    t.OperatorAssociativity = OperatorAssociativity.Right;
                    break;
                case SemanticTokenType.Negation:
                    t.Precedence = 8;
                    t.OperatorAssociativity = OperatorAssociativity.Right;
                    break;
                case SemanticTokenType.Modulo:
                    t.Precedence = 7;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case SemanticTokenType.GreaterThan:
                    t.Precedence = 5;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case SemanticTokenType.GreaterThanOrEqualTo:
                    t.Precedence = 5;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case SemanticTokenType.LessThan:
                    t.Precedence = 5;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case SemanticTokenType.LessThanOrEqualTo:
                    t.Precedence = 5;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case SemanticTokenType.Equal:
                    t.Precedence = 4;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case SemanticTokenType.NotEqual:
                    t.Precedence = 4;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case SemanticTokenType.LogicalAnd:
                    t.Precedence = 3;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;
                case SemanticTokenType.LogicalOr:
                    t.Precedence = 2;
                    t.OperatorAssociativity = OperatorAssociativity.Left;
                    break;

                default:
                    throw new ExpressionParserException(String.Format("Unknown operator{0}", t));
                    break;
            }

            return t;
        }


        public void PreprocessSemantics(ref List<InputToken> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].TokenType == SemanticTokenType.Subtract)
                {
                    if (tokens.Count - 1 < i + 1)
                        throw new Exception("Unexpected end of token stream");

                    if (tokens[i + 1].TokenType == SemanticTokenType.OpenBracket)
                        tokens[i].TokenType = SemanticTokenType.UnaryMinus;

                    if (i > 0 && tokens[i - 1].OperationType == OperationType.Operator)
                        tokens[i].TokenType = SemanticTokenType.UnaryMinus;

                    if (i > 0 && tokens[i - 1].TokenType == SemanticTokenType.OpenBracket)
                        tokens[i].TokenType = SemanticTokenType.UnaryMinus;

                    if (i == 0)
                        tokens[i].TokenType = SemanticTokenType.UnaryMinus;
                }
            }
        }
    }
}
