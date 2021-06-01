using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspileTest
{
    public enum OperationType
    {
        Operand = 0,
        Operator = 1,
        FunctionCall = 2
    }

    public enum TokenDiscardPolicy
    {
        Keep = 0,
        Discard = 1
    }

    public enum IdPolicy
    {
        None = 0,
        IdPreceedsCurrentToken = 1,
        IdPostCurrentToken = 2
    }


    public enum OperatorAssociativity
    {
        None = 0,
        Left = 1,
        Right = 2
    }

    public enum SemanticTokenType
    {
        None = 0,

        // Operators
        BeginExpressionTokens = 1, // Used to determine if any tokens can be included in the expression parser
        Add = 1,
        Subtract = 2,
        Multiply = 3,
        Divide = 4,
        Assignment = 5,
        Negation = 6,
        PowerOf = 7,
        UnaryMinus = 10,
        Modulo = 11,
        GreaterThan = 12,
        LessThan = 13,
        Equal = 14,
        NotEqual = 15,
        GreaterThanOrEqualTo = 16,
        LessThanOrEqualTo = 17,
        LogicalAnd = 18,
        LogicalOr = 19,
        OpenBracket = 20,
        CloseBracket = 21,
        Whitespace = 22,
        FunctionArgumentSeperator = 23,

        StringValue = 24,
        VariableName = 25,
        DecimalLiteral32 = 26,
        FunctionCall = 27,
        Symbol = 28, // Is this used?
        DecimalLiteral16 = 29,
        DecimalLiteral8 = 30,
        LiteralTrue = 31,
        LiteralFalse = 32,

        EndExpressionTokens = 33,


        // Labels
        LabelGossipScript = 40,
        PageLabel = 41,
        // Nodes

        // Placeholders
        
        OpenCurlyBrace = 132,
        CloseCurlyBrace = 133,

        // Operands
        // IntegerLiteral = 160,

        StartStream = 255,
        NodeInBuilt = 256,
        NodeParameter = 257,
        Comment = 512,

        // Reserved words
        TypeFlag = 700,
        TypeInteger = 701,
        ScopeGlobal = 750,
        ScopeScript = 751,

        Identifier = 1024
    }
}
