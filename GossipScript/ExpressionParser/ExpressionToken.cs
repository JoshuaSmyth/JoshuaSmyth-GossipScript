using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TranspileTest.Parser
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ExpressionData
    {
        [FieldOffset(0)]
        public float Float;
        [FieldOffset(0)]
        public int Int;
        [FieldOffset(0)]
        public Guid Guid;   // 16 Bytes
    }

    public class ExpressionToken // Ecapsulates the idea of an operator and an operand
    {
        public SemanticTokenType TokenType;

        public OperationType OperationType;

        public OperatorAssociativity OperatorAssociativity;

        public Int32 Precedence; // Higher evaluates first

        public ExpressionData Data;

       // public string stringValue; // TODO Work something else out?

        public bool IsOperator()
        {
            return OperationType == OperationType.Operator;
        }

        public bool IsValue()
        {
            return OperationType == OperationType.Operand;
        }

        public bool IsVariableNumberType()
        {
            // TODO Do we need to see if a semantic check can be done to assert variable name is a number?
            return IsValue() && (TokenType == SemanticTokenType.VariableName);
        }

        public bool IsNumber()
        {
            return IsValue() && (TokenType == SemanticTokenType.DecimalLiteral32);
        }

        public bool IsBracket()
        {
            return (TokenType == SemanticTokenType.OpenBracket || TokenType == SemanticTokenType.CloseBracket);
        }

        public bool IsFunction()
        {
            return TokenType == SemanticTokenType.FunctionCall;
        }

        public bool IsFunctionArgumentSeperator()
        {
            return TokenType == SemanticTokenType.FunctionArgumentSeperator;
        }
    }
}
