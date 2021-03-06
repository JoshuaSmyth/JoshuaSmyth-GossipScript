﻿using ExpressionParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TranspileTest.ExpressionParser;
using TranspileTest.Nodes;
using TranspileTest.Parser;

namespace TranspileTest
{
    public class TokenStreamSet
    {
        public List<TokenStream> TokenStreams = new List<TokenStream>();

        public TokenStreamSet()
        {

        }

        public TokenStreamSet(TokenStream tokenStream)
        {
            TokenStreams.Add(tokenStream);
        }
    }


    public class TokenStream
    {
        private List<InputToken> inputTokens;

        int index = 0;

        public TokenStream(List<InputToken> tokens)
        {
            inputTokens = tokens ?? throw new ArgumentNullException();
        }

        public InputToken Pop()
        {
            var rv = GetCurrent();
            AdvanceNext();
            return rv;
        }

        public int Count()
        {
            return inputTokens.Count;
        }

        public InputToken GetPrevious()
        {
            var subindex = index - 2; // This is because the popped value is -1 so we want -2
            if (subindex < 0)
            {
                return null;
            }
            return inputTokens[subindex];
        }

        public InputToken GetCurrent()
        {
            if (index >= inputTokens.Count)
            {
                return null;
            }
            return inputTokens[index];
        }

        public string Stringify()
        {
            // Also prettify
            // Add 4 spaces per tab

            // Keep a tab stack
            var indent = 0;

            var sb = new StringBuilder();

            for(int i=0; i<inputTokens.Count; i++)
            {
                var t = inputTokens[i];

                // Pretoken
                {
                    if (t.TokenType == SemanticTokenType.LabelGossipScript)
                    {
                        sb.Append(Environment.NewLine);
                    }

                    if (t.TokenType == SemanticTokenType.OpenCurlyBrace)
                    {
                        // Except if previous was a comment
                        if (i - 1 > 0 && inputTokens[i - 1].TokenType == SemanticTokenType.Comment)
                        {
                            // NOOP
                        }
                        else
                        {
                            sb.Append(Environment.NewLine);
                        }
                    }

                    if (t.TokenType == SemanticTokenType.CloseCurlyBrace)
                    {
                        // Except if previous was a comment
                        if (i - 1 > 0 && inputTokens[i - 1].TokenType == SemanticTokenType.Comment)
                        {
                            // NOOP
                        }
                        else
                        {
                            sb.Append(Environment.NewLine);
                        }
                        indent--;
                    }


                    // Apply the indentation
                    if (t.TokenType == SemanticTokenType.NodeParameter)
                    {
                        sb.Append(" ");
                    }
                    else
                    {
                        // If the previous token was 
                        if (i - 1 > 0 && inputTokens[i - 1].TokenType == SemanticTokenType.NodeParameter)
                        {
                            // NOOP
                        }
                        else
                        {
                            if (t.TokenType == SemanticTokenType.StringValue)
                            {
                                // NOOP
                            }
                            else if (t.TokenType == SemanticTokenType.Identifier)
                            {
                                if (i - 1 > 0 && inputTokens[i - 1].TokenType == SemanticTokenType.StringValue)
                                {
                                    sb.Append(Environment.NewLine);
                                    sb.Append(Environment.NewLine);
                                }
                                //else
                                {
                                    for (int s = 0; s < indent; s++)
                                    {
                                        sb.Append("    ");
                                    }
                                }
                            }
                            else
                            {
                                for (int s = 0; s < indent; s++)
                                {
                                    sb.Append("    ");
                                }
                            }
                        }
                    }

                    if (t.TokenType == SemanticTokenType.OpenCurlyBrace)
                    {
                        indent++;
                    }
                }

                // Print Token
                {
                    sb.Append(t.TokenValue);
                }

                // Post Token
                {
                    if (t.TokenType == SemanticTokenType.OpenCurlyBrace)
                    {
                        sb.Append(Environment.NewLine);
                    }


                    // If the next token is a } don't do this
                    if (i + 1 < inputTokens.Count && inputTokens[i + 1].TokenType == SemanticTokenType.CloseCurlyBrace)
                    {

                    }
                    else
                    {
                        if (t.TokenType == SemanticTokenType.CloseCurlyBrace)
                        {
                            sb.Append(Environment.NewLine);
                            sb.Append(Environment.NewLine);
                        }
                    }

                    if (t.TokenType == SemanticTokenType.Identifier)
                    {
                        if (i - 1 > 0 && inputTokens[i - 1].TokenType != SemanticTokenType.NodeParameter)
                        {
                            sb.Append(Environment.NewLine);
                        }
                    }
                }
            }

            return sb.ToString();
        }

        public InputToken PeekNext()
        {
            if (index+1 >= inputTokens.Count)
            {
                return null;
            }
            return inputTokens[index+1];
        }

        public void AdvanceNext()
        {
            index++;
        }

        public bool TokensRemaining()
        {
            return index < inputTokens.Count;
        }

        public List<InputToken> ToList()
        {
            return inputTokens.ToList();
        }
    }

    public class ScriptParser
    {
        private Tokenizer m_tokenizer = new Tokenizer();
        private static ExpressionCompiler expressionParser = new ExpressionCompiler(new HostCallTable());

        
        public TokenStreamSet AssignIdentifiers(TokenStreamSet tokenStreamSet)
        {
            var existingIdentifiers = new HashSet<uint>();
            foreach(var tokenStream in tokenStreamSet.TokenStreams)
            {
                foreach(var token in tokenStream.ToList())
                {
                    if (token.TokenType == SemanticTokenType.Identifier)
                    {
                        var substring = token.TokenValue.Substring(1, 8);
                        var value = UInt32.Parse(substring, System.Globalization.NumberStyles.HexNumber);

                        if (existingIdentifiers.Contains(value))
                        {
                            throw new Exception("Duplicate identifier found in scripts!");
                        }
                        else
                        {
                            existingIdentifiers.Add(value);
                        }
                    }
                }
            }

            var r = new Random();

            // Assume we got here, let's check if we need to add new identifiers
            var previousToken = new InputToken();
            var previousPreviousToken = new InputToken();

            // Lets copy
            var newTokenStreamSet = new TokenStreamSet();

            foreach (var tokenStream in tokenStreamSet.TokenStreams)
            {
                var newTokenStream = new List<InputToken>();


                var tokenList = tokenStream.ToList();
                for(int i=0; i<tokenList.Count; i++)
                {
                    var token = tokenList[i];

                    bool shouldGenerateId = false;
                    if (token.IdPolicy == IdPolicy.IdPreceedsCurrentToken)
                    {
                        {
                            if (previousToken.TokenType == SemanticTokenType.Identifier)
                            {
                                // No need
                            }
                            else
                            {
                                shouldGenerateId = true;
                            }
                        }
                    }

                    if (token.IdPolicy == IdPolicy.IdPostCurrentToken)
                    {
                        if (i+1 < tokenList.Count)
                        {
                            var nextToken = tokenList[i + 1];
                            if (nextToken.TokenType == SemanticTokenType.Identifier)
                            {
                                // No need
                            }
                            else
                            {
                                shouldGenerateId = true;
                            }
                         }
                        else
                        {
                            throw new Exception("Unexpected end of tokenstream");
                        }
                    }

                    // Add current token
                    if (shouldGenerateId)
                    {
                        // Generate id
                        uint partA = (uint)r.Next(1 << 16);
                        uint PartB = (uint)r.Next(1 << 16);
                        uint newId = (partA << 16) | PartB;

                        // Try again if id already exists
                        // Note: This would infinite loop if the hashset is full, but that's a lot of identifiers!
                        int tryCount = 0;
                        const int MaxRetries = 12;
                        while (existingIdentifiers.Contains(newId))
                        {
                            partA = (uint)r.Next(1 << 16);
                            PartB = (uint)r.Next(1 << 16);
                            newId = (partA << 16) | PartB;

                            tryCount++;
                            if (tryCount > MaxRetries)
                            {
                                throw new Exception("Cannot Generated Valid Identifier: Exceeded max number of retries");
                            }
                        }


                        var generatedToken = new InputToken(new Regex("\\[[0-9A-Fa-f]{8}\\]"), IdPolicy.None, SemanticTokenType.Identifier, OperationType.Operand);
                        generatedToken.TokenValue = String.Format("[{0}]", newId.ToString("x8"));


                        // todo add whitespace?
                        if (token.IdPolicy == IdPolicy.IdPreceedsCurrentToken)
                        {
                            newTokenStream.Add(generatedToken);
                            newTokenStream.Add(token);

                            previousPreviousToken = generatedToken;
                            previousToken = token;
                        }
                        else if (token.IdPolicy == IdPolicy.IdPostCurrentToken)
                        {
                            newTokenStream.Add(token);
                            newTokenStream.Add(generatedToken);
                     
                            previousPreviousToken = token;
                            previousToken = generatedToken;
                        }
                        else
                        {
                            throw new Exception("Unhanded Token IdPolicy");
                        }
                    }
                    else
                    {
                        newTokenStream.Add(token);


                        previousPreviousToken = previousToken;
                        previousToken = token;
                    }
                }

                newTokenStreamSet.TokenStreams.Add(new TokenStream( newTokenStream ));
            }


            return newTokenStreamSet;
        }



        public ScriptParser()
        {
            m_tokenizer.AddToken("@GossipScript",  IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.LabelGossipScript);
            m_tokenizer.AddToken("{", IdPolicy.None, SemanticTokenType.OpenCurlyBrace);
            m_tokenizer.AddToken("}", IdPolicy.None, SemanticTokenType.CloseCurlyBrace);

            // Page Label
            m_tokenizer.AddToken("@[a-zA-Z_][a-zA-Z_0-9]*", IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.PageLabel);

            // String Literal
            m_tokenizer.AddToken("\"((\\.)|[^\\\\\"])*\"", IdPolicy.None, SemanticTokenType.StringValue);

            m_tokenizer.AddToken("flag", IdPolicy.None, SemanticTokenType.TypeFlag);
            m_tokenizer.AddToken("int", IdPolicy.None, SemanticTokenType.TypeInteger);

            m_tokenizer.AddToken("global", IdPolicy.None, SemanticTokenType.ScopeGlobal);
            m_tokenizer.AddToken("local", IdPolicy.None, SemanticTokenType.ScopeScript);

            m_tokenizer.AddToken("actor:", IdPolicy.None, SemanticTokenType.NodeParameter);

            // TODO Change this to IdPostCurrentToken
            m_tokenizer.AddToken("text:", IdPolicy.IdPostCurrentToken, SemanticTokenType.NodeParameter);
            m_tokenizer.AddToken("position:", IdPolicy.None, SemanticTokenType.NodeParameter);
            m_tokenizer.AddToken("node:", IdPolicy.None, SemanticTokenType.NodeParameter);
            m_tokenizer.AddToken("remove-on-select:", IdPolicy.None, SemanticTokenType.NodeParameter);
            m_tokenizer.AddToken("exit-on-select:", IdPolicy.None, SemanticTokenType.NodeParameter);
            m_tokenizer.AddToken("time:", IdPolicy.None, SemanticTokenType.NodeParameter);

            // These are all valid tokens for expressions

            m_tokenizer.AddToken("true", IdPolicy.None, SemanticTokenType.LiteralTrue, OperationType.Operand);
            m_tokenizer.AddToken("false", IdPolicy.None, SemanticTokenType.LiteralFalse, OperationType.Operand);
            m_tokenizer.AddToken(@"\$[a-zA-Z_][a-zA-Z_0-9]*", IdPolicy.None, SemanticTokenType.VariableName, OperationType.Operand);
            m_tokenizer.AddToken("==", IdPolicy.None, SemanticTokenType.Equal);
            m_tokenizer.AddToken("&&", IdPolicy.None, SemanticTokenType.LogicalAnd);
            m_tokenizer.AddToken(new Regex(Regex.Escape("||")), IdPolicy.None, SemanticTokenType.LogicalOr);
            m_tokenizer.AddToken(">=", IdPolicy.None, SemanticTokenType.GreaterThanOrEqualTo);
            m_tokenizer.AddToken("<=", IdPolicy.None, SemanticTokenType.LessThanOrEqualTo);
            m_tokenizer.AddToken("==", IdPolicy.None, SemanticTokenType.Equal);
            m_tokenizer.AddToken("!=", IdPolicy.None, SemanticTokenType.NotEqual);
            m_tokenizer.AddToken(">", IdPolicy.None, SemanticTokenType.GreaterThan);
            m_tokenizer.AddToken("<", IdPolicy.None, SemanticTokenType.LessThan);
            m_tokenizer.AddToken(new Regex(Regex.Escape("/")), IdPolicy.None, SemanticTokenType.Divide);
            m_tokenizer.AddToken("%", IdPolicy.None, SemanticTokenType.Modulo);
            m_tokenizer.AddToken(new Regex(Regex.Escape("*")), IdPolicy.None, SemanticTokenType.Multiply);
            m_tokenizer.AddToken(new Regex(Regex.Escape("+")), IdPolicy.None, SemanticTokenType.Add);
            m_tokenizer.AddToken("-", IdPolicy.None, SemanticTokenType.Subtract);
            m_tokenizer.AddToken("!", IdPolicy.None, SemanticTokenType.Negation);
            m_tokenizer.AddToken(new Regex(Regex.Escape("(")), IdPolicy.None, SemanticTokenType.OpenBracket, OperationType.Operand);
            m_tokenizer.AddToken(new Regex(Regex.Escape(")")), IdPolicy.None, SemanticTokenType.CloseBracket, OperationType.Operand);
            
            
            // End valid for expressions



            m_tokenizer.AddToken("name:", IdPolicy.None, SemanticTokenType.NodeParameter);
            m_tokenizer.AddToken("type:", IdPolicy.None, SemanticTokenType.NodeParameter);
            m_tokenizer.AddToken("value:", IdPolicy.None, SemanticTokenType.NodeParameter);
            m_tokenizer.AddToken("scope:", IdPolicy.None, SemanticTokenType.NodeParameter);
            m_tokenizer.AddToken("default:", IdPolicy.None, SemanticTokenType.NodeParameter);

            m_tokenizer.AddToken("expr:", IdPolicy.None, SemanticTokenType.NodeParameter);
            m_tokenizer.AddToken("var:", IdPolicy.None, SemanticTokenType.NodeParameter);


            // Nodes
            m_tokenizer.AddToken("say", IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("call-page", IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("return", IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("once-only", IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("case-true", IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("case-false", IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("show-options", IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("option", IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("wait", IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("print", IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);

            m_tokenizer.AddToken("parallel", IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("block", IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);

            m_tokenizer.AddToken("if", IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("def", IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("set-var", IdPolicy.IdPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
        }

        // Assigns Ids and rewrites the token stream
        public TokenStream Identify(string script)
        {
            // TODO Create Script Identifier class
            var tokens = m_tokenizer.Tokenize(script);
            var tokenStream = new TokenStream(tokens);


            return tokenStream;
        }


        public TokenStream TokenizeFile(string filename, bool discardWhitespace = true, bool discardComments = true)
        {
            var text = File.ReadAllText(filename);
            return this.TokenizeString(text, discardWhitespace, discardComments);
        }

        public TokenStream TokenizeString(string script, bool discardWhitespace=true, bool discardComments=true)
        {
            var tokens = m_tokenizer.Tokenize(script, discardWhitespace, discardComments);
            var tokenStream = new TokenStream(tokens);

            return tokenStream;
        }

        public ScriptNode ParseScript(ScriptProgram program, TokenStream tokenStream)
        {
            return processStream(program, tokenStream);
        }

        public ScriptNode ParseScript(ScriptProgram program, string script)
        {
            var tokens = TokenizeString(script);

            return ParseScript(program, tokens);
        }

        private ScriptNode processStream(ScriptProgram program, TokenStream tokenStream)
        {
            var rv = new ScriptNode();

            processScriptHeader(program, rv, tokenStream);
            processPages(program, rv, tokenStream);
            
            // Tokenstream should be at the end of the stream
            if (tokenStream.TokensRemaining())
            {
                throw new Exception("Did not reach end of stream");
            }
            return rv;
        }


        private static uint ConvertIdentifierValueToInt(string id)
        {
            return uint.Parse(id.Substring(1,8), System.Globalization.NumberStyles.HexNumber);
        }

        private void processScriptHeader(ScriptProgram program, ScriptNode root, TokenStream stream)
        {
            while (stream.GetCurrent().TokenType != SemanticTokenType.PageLabel)
            {
                // If we have defined a global var process it
                var currentToken = stream.GetCurrent();
                if (currentToken.TokenType == SemanticTokenType.NodeInBuilt)
                {
                    if (currentToken.TokenValue == "def")
                    {
                        stream.Pop();

                        // Load name/scope/default value
                        ParseHeaderNodeDefNode(program, root, stream);
                    }
                    else
                    {
                        throw new Exception("Unknown node:" + currentToken.TokenValue);
                    }
                }
                else
                {
                    stream.AdvanceNext();
                }
                if (currentToken.TokenType == SemanticTokenType.Identifier)
                {
                    root.Id = ConvertIdentifierValueToInt(currentToken.TokenValue);
                }

                // We hit the first page
                currentToken = stream.GetCurrent();
                if (currentToken.TokenType == SemanticTokenType.PageLabel)
                {
                    break;
                }

               // stream.AdvanceNext();
            }
        }

        private static void ParseHeaderNodeDefNode(ScriptProgram program, ScriptNode root, TokenStream stream)
        {
            string variablename = "";
            string defaultValue = "";   // To be converted later
            Guid variableId = Guid.Empty;
            ScriptVariableType variableType = ScriptVariableType.Unknown;
            ScriptVariableScope variableScope = ScriptVariableScope.Unknown;

            var parameter = stream.Pop();
            while (parameter.TokenType == SemanticTokenType.NodeParameter)
            {
                //var argumentValue = stream.Pop();
                if (parameter.TokenValue == "name:")
                {
                    var value = stream.Pop();
                    variablename = value.TokenValue;
                }
                else if (parameter.TokenValue == "type:")
                {
                    var value = stream.Pop();
                    if (value.TokenValue == "flag")
                    {
                        variableType = ScriptVariableType.Flag;
                    }
                    else if (value.TokenValue == "int")
                    {
                        variableType = ScriptVariableType.Integer;
                    }
                    else
                    {
                        throw new Exception("Unsupported variable type:" + value.TokenValue);
                    }
                }
                else if (parameter.TokenValue == "scope:")
                {
                    var value = stream.Pop();
                    if (value.TokenValue == "global")
                    {
                        variableScope = ScriptVariableScope.Global;
                    }
                    else if (value.TokenValue == "local")
                    {
                        variableScope = ScriptVariableScope.Local;
                    }
                    else
                    {
                        throw new Exception("Unsupported scope:" + value);
                    }
                }
                else if (parameter.TokenValue == "default:")
                {
                    var value = stream.Pop();
                    defaultValue = value.TokenValue;
                }
                else
                {
                    throw new Exception("Unknown parameter:" + parameter.TokenValue);
                }


                var nexttype = stream.GetCurrent().TokenType;
                if (nexttype != SemanticTokenType.NodeParameter)
                {
                    break;
                }
                parameter = stream.Pop();
            }
            
            // TODO Look up guid properly
            Console.WriteLine("TODO Look up guid for variable name");
            if (variableId == Guid.Empty)
            {
                variableId = Guid.NewGuid();
            }

            switch (variableType)
            {
                case ScriptVariableType.Integer:
                    {
                        var value = int.Parse(defaultValue);
                        if (variableScope == ScriptVariableScope.Global)
                        {
                            program.GlobalVariables.AddVariable(variableId, variableType, value, variablename);
                        }
                        else if (variableScope == ScriptVariableScope.Local)
                        {
                            root.LocalVariables.AddVariable(variableId, variableType, value, variablename);
                        }
                        break;
                    }
                case ScriptVariableType.Flag:
                    {
                        var value = bool.Parse(defaultValue);
                        if (variableScope == ScriptVariableScope.Global)
                        {
                            program.GlobalVariables.AddVariable(variableId, variableType, value, variablename);
                        }
                        else if (variableScope == ScriptVariableScope.Local)
                        {
                            root.LocalVariables.AddVariable(variableId, variableType, value, variablename);
                        }
                        else
                        {
                            throw new Exception("Unsupported Variable");
                        }
                        break;
                    }
                default:
                    throw new Exception("Unsupported VariableType:" + variableType);
            }
        }

        private void processPages(ScriptProgram program, Node root, TokenStream stream)
        {
            // TODO While loop here:
            while (stream.TokensRemaining() && stream.PeekNext().TokenType != SemanticTokenType.PageLabel)
            {
                var token = stream.Pop();

                // Skip the id and assume we can fix it up later.
                if (token.TokenType == SemanticTokenType.Identifier)
                {
                    token = stream.Pop();
                }


                if (token.TokenType == SemanticTokenType.PageLabel)
                {
                    var id = stream.GetPrevious();
                    
                    var page = root.AddChildNode(new PageNode(token.TokenValue));
                    if (id.TokenType == SemanticTokenType.Identifier)
                    {
                        page.Id = ConvertIdentifierValueToInt(id.TokenValue);
                    } 
                    else
                    {
                        // TODO Should we throw an error here?
                    }
                    
                    ParseNodePage(program, page, stream);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }


        private void ParseNodePage(ScriptProgram scriptProgram, Node root, TokenStream stream)
        {
            var token = stream.Pop();
            var tokenType = token.TokenType;
            if (tokenType == SemanticTokenType.OpenCurlyBrace)
            {
                // Ignore
            }
            else
            {
                // Expect open bracket
            }

            // Process the rest of the page
            while (stream.TokensRemaining())
            {
                token = stream.Pop();
                tokenType = token.TokenType;

                // Skip whitespace
                if (tokenType == SemanticTokenType.Whitespace ||
                    tokenType == SemanticTokenType.OpenCurlyBrace)
                {
                    continue;
                }
                if (tokenType == SemanticTokenType.Identifier)
                {
                    continue;
                }

                if (tokenType == SemanticTokenType.CloseCurlyBrace)
                {
                    return;
                }
                else if (tokenType == SemanticTokenType.NodeInBuilt)
                {
                    ParseNode(scriptProgram, root, token, stream);
                }
                else
                {
                    throw new NotImplementedException("Unexpected Token Type:"+ tokenType);
                }
            }
        }

        private static void ParseNode(ScriptProgram program, Node root, InputToken currentToken, TokenStream stream)
        {
            // Special Id handling
            if (currentToken.TokenType == SemanticTokenType.Identifier)
            {
                // Skip the identifier and assume it will be correctly read later.
                currentToken = stream.Pop();
            }
            

            if (currentToken.TokenValue == "say")
            {
                ParseNodeSay(root, stream);
            }
            else if (currentToken.TokenValue == "option")
            {
                ParseNodeOption(program, root, stream);
            }
            else if (currentToken.TokenValue == "call-page")
            {
                ParseNodeCallPage(root, stream);
            }
            else if (currentToken.TokenValue == "return")
            {
                root.AddChildNode(new ReturnNode());
            }
            else if (currentToken.TokenValue == "once-only")
            {
                ParseNodeOnceOnly(program, root, stream);
            }
            else if (currentToken.TokenValue == "case-true")
            {
                ParseNodeCaseTrue(program, root, stream);
            }
            else if (currentToken.TokenValue == "case-false")
            {
                ParseCaseFalseNode(program, root, stream);
            }
            else if (currentToken.TokenValue == "show-options")
            {
                ParseShowOptions(program, root, stream);
            }
            else if (currentToken.TokenValue == "wait")
            {
                ParseWait(root, stream);
            }
            else if (currentToken.TokenValue == "print")
            {
                ParseNodePrint(root, stream);
            }
            else if (currentToken.TokenValue == "parallel")
            {
                ParseNodeParallel(program, root, stream);
            }
            else if (currentToken.TokenValue == "block")
            {
                ParseNodeBlock(program, root, stream);
            }
            else if (currentToken.TokenValue == "set-var")
            {
                ParseNodeSetVariable(program, root, stream);
            }
            else if (currentToken.TokenValue == "if")
            {
                ParseNodeIf(program, root, stream);
            }
            else
            {
                throw new NotImplementedException("Cannot parse Unknown Node:" + currentToken.TokenValue);
            }
        }

        private static void ParseNodeParallel(ScriptProgram scriptProgram, Node root, TokenStream stream)
        {
            var node = new ParallelNode();
            root.AddChildNode(node);

            ParseNodeChildren(scriptProgram, node, stream);

            //throw new NotImplementedException("TODO Check node parameters and parse the children");
        }

        private static void ParseNodeBlock(ScriptProgram scriptProgram, Node root, TokenStream stream)
        {
            var node = new BlockNode();
            root.AddChildNode(node);

            ParseNodeChildren(scriptProgram, node, stream);

            //throw new NotImplementedException("TODO Check node parameters and parse the children");
        }

        private static void ParseNodeOption(ScriptProgram scriptProgram, Node root, TokenStream stream)
        {
            var node = new OptionNode();
            root.AddChildNode(node);

            // TODO Read any node parameters
            // e.g remove-on-select
            while (stream.GetCurrent().TokenType == SemanticTokenType.NodeParameter)
            {
                var parameter = stream.Pop();
                if (parameter.TokenValue == "exit-on-select:")
                {
                    var value = stream.Pop();
                    if (value.TokenType == SemanticTokenType.LiteralTrue)
                    {
                        node.ReturnOnSelect = true;
                    }
                    else if (value.TokenType == SemanticTokenType.LiteralFalse)
                    {
                        node.ReturnOnSelect = false;
                    }
                    else
                    {
                        throw new Exception("Invalid token parameter value");
                    }
                }
                else if (parameter.TokenValue == "remove-on-select:")
                {
                    var value = stream.Pop();
                    if (value.TokenType == SemanticTokenType.LiteralTrue)
                    {
                        node.RemoveOnSelect = true;
                    }
                    else if (value.TokenType == SemanticTokenType.LiteralFalse)
                    {
                        node.RemoveOnSelect = false;
                    }
                    else
                    {
                        throw new Exception("Invalid token parameter value");
                    }
                }
                else if (parameter.TokenValue == "text:")
                {
                    var value = stream.Pop();
                    if (value.TokenType == SemanticTokenType.Identifier)
                    {
                        node.Id = ConvertIdentifierValueToInt(value.TokenValue);
                        parameter = stream.Pop();
                        node.Text = parameter.TokenValue;
                    }
                    else if (value.TokenType == SemanticTokenType.StringValue)
                    {
                        node.Text = value.TokenValue;
                    }
                    else
                    {
                        throw new Exception("Invalid Token parameter value");
                    }
                }
                else
                {
                    throw new Exception("Unknown parameter:" + parameter.TokenValue);
                }
            }

            ParseNodeChildren(scriptProgram, node, stream);

            //throw new NotImplementedException("TODO Check node parameters and parse the children");
        }

        private static Expression ParseExpression(ScriptVariableTable scriptVariableTable, TokenStream stream)
        {
            Console.WriteLine("Parse expression needs to resolve any variable lookups");
            Console.WriteLine("Parse expression also needs to work out the return type");

            var expressionTokens = new List<InputToken>();
            var currentToken = stream.PeekNext();

            while ((int)currentToken.TokenType >= (int)SemanticTokenType.BeginExpressionTokens &&
                (int)currentToken.TokenType <= (int)SemanticTokenType.EndExpressionTokens)
            {
                expressionTokens.Add(stream.Pop());
                currentToken = stream.GetCurrent();
            }

            var rv = new Expression();
            rv.Instructions = expressionParser.ConvertToReversePolishNotation(scriptVariableTable, expressionTokens);

            return rv;
        }

        private static void ParseNodeSetVariable(ScriptProgram scriptProgram, Node root, TokenStream stream)
        {
            var node = new SetVarNode();
            root.AddChildNode(node);

            string name = "";
            Expression value = null; // TODO This will be parsed as an expression
            string scope = "";

            while (stream.GetCurrent().TokenType == SemanticTokenType.NodeParameter)
            {
                var parameter = stream.Pop();
                if (parameter.TokenValue == "name:")
                {
                    name = stream.Pop().TokenValue;
                }
                else if (parameter.TokenValue == "value:")
                {
                    value = ParseExpression(scriptProgram.GlobalVariables, stream);

                }
                else if (parameter.TokenValue == "scope:")
                {
                    scope = stream.Pop().TokenValue;
                }
                else
                {
                    throw new Exception("Unknown parameter:" + parameter.TokenValue);
                }
            }

            // TODO Add compile type checking for variable existance and scope
            node.expression = value;
            node.variableId = name;
        }

        private static void ParseWait(Node root, TokenStream stream)
        {
            var node = new WaitNode();
            root.AddChildNode(node);

            while (stream.GetCurrent().TokenType == SemanticTokenType.NodeParameter)
            {
                var parameter = stream.Pop();
                if (parameter.TokenValue == "time:")
                {
                    var value = stream.Pop();
                    if (value.TokenType == SemanticTokenType.DecimalLiteral16 ||
                        value.TokenType == SemanticTokenType.DecimalLiteral32 ||
                        value.TokenType == SemanticTokenType.DecimalLiteral8)
                    {
                        node.WaitTimeMilliseconds = Int32.Parse(value.TokenValue);
                    }
                    else
                    {
                        throw new Exception("Invalid token parameter value");
                    }
                }
                else
                {
                    throw new Exception("Unknown parameter:" + parameter.TokenValue);
                }
            }
        }


        private static void ParseNodeIf(ScriptProgram program, Node root, TokenStream stream)
        {
            var node = new ConditionalExpressionNode();
            root.AddChildNode(node);

            while (stream.GetCurrent().TokenType == SemanticTokenType.NodeParameter)
            {
                var parameter = stream.Pop();
                if (parameter.TokenValue == "expr:")
                {
                    node.Expression = ParseExpression(program.GlobalVariables, stream);
                }
                else
                {
                    throw new Exception("Invalid token parameter value");
                }
            }
            
            ParseNodeChildren(program, node, stream);
        }
        
        private static void ParseShowOptions(ScriptProgram scriptProgram, Node root, TokenStream stream)
        {
            var node = new ShowOptionsNode();
            root.AddChildNode(node);

            // TODO Read any node parameters
            // e.g remove-on-select
            while (stream.GetCurrent().TokenType == SemanticTokenType.NodeParameter)
            {
                var parameter = stream.Pop();
                if (parameter.TokenValue == "remove-on-select:")
                {
                    var value = stream.Pop();
                    if (value.TokenType == SemanticTokenType.LiteralTrue)
                    {
                        node.RemoveOnSelect = true;
                    }
                    else if (value.TokenType == SemanticTokenType.LiteralFalse)
                    {
                        node.RemoveOnSelect = false;
                    }
                    else
                    {
                        throw new Exception("Invalid token parameter value");
                    }
                }
                else
                {
                    throw new Exception("Unknown parameter:" + parameter.TokenValue);
                }
            }

            ParseNodeChildren(scriptProgram, node, stream);
        }

        private static void ParseCaseFalseNode(ScriptProgram scriptProgram, Node root, TokenStream stream)
        {
            var node = new ConditionalFalseNode();
            root.AddChildNode(node);

            ParseNodeChildren(scriptProgram, node, stream);
        }

        private static void ParseNodeCaseTrue(ScriptProgram scriptProgram, Node root, TokenStream stream)
        {
            var node = new ConditionalTrueNode();
            root.AddChildNode(node);
            ParseNodeChildren(scriptProgram, node, stream);
        }

        private static void ParseNodeOnceOnly(ScriptProgram scriptProgram, Node root, TokenStream stream)
        {
            var node = new OnceOnlyNode();
            root.AddChildNode(node);

            ParseNodeChildren(scriptProgram, node, stream);
        }

        private static void ParseNodeChildren(ScriptProgram program, Node node, TokenStream stream)
        {
            var nexttype = stream.Pop().TokenType;

            if (nexttype == SemanticTokenType.Identifier)
            {
                // Skip the id and assume we will parse it correctly later.
                nexttype = stream.Pop().TokenType;
            }

            if (nexttype == SemanticTokenType.OpenCurlyBrace)
            {
                // Read All children
                while (stream.GetCurrent().TokenType != SemanticTokenType.CloseCurlyBrace)
                {
                    ParseNode(program, node, stream.Pop(), stream);
                }

                // Pop close Bracket and ignore.
                stream.Pop();
            }
            else
            {
                // Or we don't have children!

                //throw new Exception("Expected Open Bracket");
            }
        }

        private static void ParseNodeCallPage(Node root, TokenStream stream)
        {
            var node = new CallPageNode();
            root.AddChildNode(node);

            var parameter = stream.Pop();
            while (parameter.TokenType == SemanticTokenType.NodeParameter)
            {
                var argumentValue = stream.Pop();
                if (parameter.TokenValue == "node:")
                {
                    node.TargetPage = argumentValue.TokenValue;
                }
                else
                {
                    throw new Exception("Unknown parameter:" + parameter.TokenValue);
                }
                
                var nexttype = stream.GetCurrent().TokenType;
                if (nexttype != SemanticTokenType.NodeParameter)
                {
                    break;
                }
                parameter = stream.Pop();
            }
        }

        private static void ParseNodePrint(Node root, TokenStream stream)
        {
            var node = new PrintNode("");
            root.AddChildNode(node);

            var parameter = stream.Pop();
            while (parameter.TokenType == SemanticTokenType.NodeParameter)
            {
                var argumentValue = stream.Pop();
                if (parameter.TokenValue == "text:")
                {
                    node.Text = argumentValue.TokenValue;
                }
                else
                {
                    throw new Exception("Unknown parameter:" + parameter.TokenValue);
                }

                var nexttype = stream.GetCurrent().TokenType;
                if (nexttype != SemanticTokenType.NodeParameter)
                {
                    break;
                }
                parameter = stream.Pop();
            }
        }

        private static void ParseNodeSay(Node root, TokenStream stream)
        {
            var node = new SayNode("", "", "");
            root.AddChildNode(node);

            var parameter = stream.Pop();
            while (parameter.TokenType == SemanticTokenType.NodeParameter)
            {
                var argumentToken = stream.Pop();


                if (parameter.TokenValue == "actor:")
                {
                    node.ActorId = argumentToken.TokenValue;
                }
                else if (parameter.TokenValue == "text:")
                {
                    if (argumentToken.TokenType == SemanticTokenType.Identifier)
                    {
                        node.Id = ConvertIdentifierValueToInt(argumentToken.TokenValue);
                        argumentToken = stream.Pop();
                        node.Text = argumentToken.TokenValue;
                    }
                    else
                    {
                        node.Text = argumentToken.TokenValue;
                    }
                }
                else if (parameter.TokenValue == "position:")
                {
                    node.Text = argumentToken.TokenValue;
                }
                else
                {
                    throw new Exception("Unknown parameter:" + parameter.TokenValue);
                }

                var nexttype = stream.GetCurrent().TokenType;
                if (nexttype != SemanticTokenType.NodeParameter)
                {
                    break;
                }
                parameter = stream.Pop();
            }
        }

    }
}
