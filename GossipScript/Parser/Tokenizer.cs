using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TranspileTest;
using TranspileTest.Parser;

namespace TranspileTest
{
    public class Tokenizer
    {
        readonly List<InputToken> m_Tokens = new List<InputToken>();

        public Tokenizer()
        {
            // #.*$
            m_Tokens.Add(new InputToken(new Regex(@"\#.*$", RegexOptions.Multiline), IdPolicy.None, SemanticTokenType.Comment, OperationType.Operand, TokenDiscardPolicy.Discard));
  
            m_Tokens.Add(new InputToken(new Regex(@"\s+"), IdPolicy.None, SemanticTokenType.Whitespace, OperationType.Operand, TokenDiscardPolicy.Discard));


            m_Tokens.Add(new InputToken(new Regex("[0-9]+([.,][0-9]+)?"), IdPolicy.None, SemanticTokenType.DecimalLiteral32, OperationType.Operand));
            m_Tokens.Add(new InputToken(new Regex(","), IdPolicy.None, SemanticTokenType.FunctionArgumentSeperator, OperationType.Operand));

            // Match Id
            m_Tokens.Add(new InputToken(new Regex("\\[[0-9A-Fa-f]{8}\\]"), IdPolicy.None, SemanticTokenType.Identifier, OperationType.Operand));

        }

        public void AddToken(Regex match, IdPolicy IdPolicy, SemanticTokenType tokenType, OperationType operationType = OperationType.Operator, TokenDiscardPolicy discardPolicy = TokenDiscardPolicy.Keep)
        {
            m_Tokens.Add(new InputToken(match, IdPolicy, tokenType, operationType, discardPolicy)); // Add tokens in order of precedence
        }

        public void AddToken(string stringToMatch, IdPolicy idPolicy, SemanticTokenType tokenType, OperationType operationType=OperationType.Operator, TokenDiscardPolicy discardPolicy = TokenDiscardPolicy.Keep)
        {
            m_Tokens.Add(new InputToken(new Regex(stringToMatch, RegexOptions.IgnoreCase), idPolicy, tokenType, operationType, discardPolicy)); // Add tokens in order of precedence
        }

        public List<InputToken> Tokenize(String source, bool ApplyDiscardPolicy=true)
        {
            var rv = new List<InputToken>();

            var currentIndex = 0;
            while (currentIndex < source.Length)
            {
                var foundMatch = false;
                foreach (var token in m_Tokens)
                {
                    var match = token.Regex.Match(source, currentIndex);
                    if (match.Success && (match.Index - currentIndex) == 0)
                    {
                        if (token.DiscardPolicy == TokenDiscardPolicy.Keep || ApplyDiscardPolicy == false)
                        {
                            rv.Add(new InputToken(token.Regex, token.IdPolicy, token.TokenType, token.OperationType, token.DiscardPolicy) { TokenValue = match.Value });
                        }

                        currentIndex += match.Length;
                        foundMatch = true;
                        break;
                    }
                }

                if (foundMatch == false)
                {
                    // Get a good-enough error token
                    var errortoken = "";
                    var errorIndex = currentIndex;
                    while (currentIndex < source.Length && currentIndex < errorIndex + 32)
                    {
                        if (source[currentIndex] == '\r' ||
                            source[currentIndex] == '\n')
                        {
                            break;
                        }
                        errortoken += source[currentIndex];
                        currentIndex++;
                    }

                    throw new ExpressionParserException("Unknown token: " + errortoken);
                }
            }
            return rv;
        }



        public void RegisterSymbol(string symbolName)
        {
            var rg = new Regex(symbolName);
            foreach (var token in m_Tokens)
            {
                if (token.TokenType == SemanticTokenType.Symbol)
                {
                    if (token.Regex.ToString() == rg.ToString())
                        return;
                }
            }

            m_Tokens.Add(new InputToken(rg, IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.Symbol, OperationType.Operand));
        }

        public void ClearAllSymbols()
        {
            for (int i = 0; i < m_Tokens.Count; i++)
            {
                if (m_Tokens[i].TokenType == SemanticTokenType.Symbol)
                {
                    m_Tokens.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
