using ExpressionParser;
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
        public List<TokenStream> inputTokens = new List<TokenStream>();
        
        public TokenStreamSet(TokenStream tokenStream)
        {
            inputTokens.Add(tokenStream);
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
            var sb = new StringBuilder();
            foreach(var t in inputTokens)
            {
                sb.Append(t.TokenValue);
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
        // TODO Add semantic analyser and host function table


        public String Identify(TokenStreamSet tokenStreamSet)
        {
            // TODO



            return "";
        }



        public ScriptParser()
        {
            m_tokenizer.AddToken("@GossipScript",  IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.LabelGossipScript);
            m_tokenizer.AddToken("{", IdPolicy.None, SemanticTokenType.OpenCurlyBrace);
            m_tokenizer.AddToken("}", IdPolicy.None, SemanticTokenType.CloseCurlyBrace);

            // Page Label
            m_tokenizer.AddToken("@[a-zA-Z_][a-zA-Z_0-9]*", IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.PageLabel);

            // String Literal
            m_tokenizer.AddToken("\"((\\.)|[^\\\\\"])*\"", IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.StringValue);

            m_tokenizer.AddToken("flag", IdPolicy.None, SemanticTokenType.TypeFlag);
            m_tokenizer.AddToken("int", IdPolicy.None, SemanticTokenType.TypeInteger);

            m_tokenizer.AddToken("global", IdPolicy.None, SemanticTokenType.ScopeGlobal);
            m_tokenizer.AddToken("local", IdPolicy.None, SemanticTokenType.ScopeScript);

            m_tokenizer.AddToken("actor:", IdPolicy.None, SemanticTokenType.NodeParameter);
            m_tokenizer.AddToken("text:", IdPolicy.None, SemanticTokenType.NodeParameter);
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
            m_tokenizer.AddToken(new Regex(Regex.Escape("||")), SemanticTokenType.LogicalOr);
            m_tokenizer.AddToken(">=", IdPolicy.None, SemanticTokenType.GreaterThanOrEqualTo);
            m_tokenizer.AddToken("<=", IdPolicy.None, SemanticTokenType.LessThanOrEqualTo);
            m_tokenizer.AddToken("==", IdPolicy.None, SemanticTokenType.Equal);
            m_tokenizer.AddToken("!=", IdPolicy.None, SemanticTokenType.NotEqual);
            m_tokenizer.AddToken(">", IdPolicy.None, SemanticTokenType.GreaterThan);
            m_tokenizer.AddToken("<", IdPolicy.None, SemanticTokenType.LessThan);
            m_tokenizer.AddToken(new Regex(Regex.Escape("/")), SemanticTokenType.Divide);
            m_tokenizer.AddToken("%", IdPolicy.None, SemanticTokenType.Modulo);
            m_tokenizer.AddToken(new Regex(Regex.Escape("*")), SemanticTokenType.Multiply);
            m_tokenizer.AddToken(new Regex(Regex.Escape("+")), SemanticTokenType.Add);
            m_tokenizer.AddToken("-", IdPolicy.None, SemanticTokenType.Subtract);
            m_tokenizer.AddToken("!", IdPolicy.None, SemanticTokenType.Negation);
            m_tokenizer.AddToken(new Regex(Regex.Escape("(")), SemanticTokenType.OpenBracket, OperationType.Operand);
            m_tokenizer.AddToken(new Regex(Regex.Escape(")")), SemanticTokenType.CloseBracket, OperationType.Operand);
            
            
            // End valid for expressions



            m_tokenizer.AddToken("name:", IdPolicy.None, SemanticTokenType.NodeParameter);
            m_tokenizer.AddToken("type:", IdPolicy.None, SemanticTokenType.NodeParameter);
            m_tokenizer.AddToken("value:", IdPolicy.None, SemanticTokenType.NodeParameter);
            m_tokenizer.AddToken("scope:", IdPolicy.None, SemanticTokenType.NodeParameter);
            m_tokenizer.AddToken("default:", IdPolicy.None, SemanticTokenType.NodeParameter);

            m_tokenizer.AddToken("expr:", IdPolicy.None, SemanticTokenType.NodeParameter);
            m_tokenizer.AddToken("var:", IdPolicy.None, SemanticTokenType.NodeParameter);


            // Nodes
            m_tokenizer.AddToken("say", IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("call-page", IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("return", IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("once-only", IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("case-true", IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("case-false", IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("show-options", IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("option", IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("wait", IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("print", IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);

            m_tokenizer.AddToken("parallel", IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("block", IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);

            m_tokenizer.AddToken("if", IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("def", IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
            m_tokenizer.AddToken("set-var", IdPolicy.IdTokenPreceedsCurrentToken, SemanticTokenType.NodeInBuilt);
        }

        // Assigns Ids and rewrites the token stream
        public TokenStream Identify(string script)
        {
            // TODO Create Script Identifier class
            var tokens = m_tokenizer.Tokenize(script);
            var tokenStream = new TokenStream(tokens);


            return tokenStream;
        }


        public TokenStream TokenizeFile(string filename)
        {
            var text = File.ReadAllText(filename);
            return this.Tokenize(text);
        }

        public TokenStream Tokenize(string script)
        {
            var tokens = m_tokenizer.Tokenize(script);
            var tokenStream = new TokenStream(tokens);

            return tokenStream;
        }

        public ScriptNode ParseScript(ScriptProgram program, TokenStream tokenStream)
        {
            return processStream(program, tokenStream);
        }

        public ScriptNode ParseScript(ScriptProgram program, string script)
        {
            var tokens = Tokenize(script);

            // TODO Run a Check Ids pass
            // BOOKMARK

            return ParseScript(program, tokens);
        }

        private ScriptNode processStream(ScriptProgram program, TokenStream tokenStream)
        {
            var rv = new ScriptNode();

            processScriptHeader(program, rv, tokenStream);
            // TODO Copy the variables over to the program

            processPages(program, rv, tokenStream);
            processIds(rv, tokenStream);

            // Tokenstream should be at the end of the stream
            if (tokenStream.TokensRemaining())
            {
                throw new Exception("Did not reach end of stream");
            }
            return rv;
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
                var tokenType = token.TokenType;
                if (tokenType == SemanticTokenType.PageLabel)
                {
                    var page = root.AddChildNode(new PageNode(token.TokenValue));
                    ParseNodePage(program, page, stream);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        private void processIds(Node root, TokenStream stream)
        {
            // TODO
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
                    throw new NotImplementedException();
                }
            }
        }

        private static void ParseNode(ScriptProgram program, Node root, InputToken currentToken, TokenStream stream)
        {
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
                    if (value.TokenType == SemanticTokenType.StringValue)
                    {
                        node.Text = value.TokenValue;
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
                var argumentValue = stream.Pop();
                if (parameter.TokenValue == "actor:")
                {
                    node.ActorId = argumentValue.TokenValue;
                }
                else if (parameter.TokenValue == "text:")
                {
                    node.Text = argumentValue.TokenValue;
                }
                else if (parameter.TokenValue == "position:")
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
    }
}
