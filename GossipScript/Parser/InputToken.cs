﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TranspileTest.Parser
{
    public class InputToken
    {
        private readonly Regex m_Regex;
        private SemanticTokenType m_TokenType;
        private readonly TokenDiscardPolicy m_DiscardPolicy;
        private readonly OperationType m_OperationType;
        private readonly IdPolicy m_IdTokenPreceedsCurrentToken;

        public SemanticTokenType TokenType
        {
            get { return m_TokenType; }
            set { m_TokenType = value; }
        }

        public Regex Regex { get { return m_Regex; } }

        public String TokenValue { get; set; }

        public OperationType OperationType
        {
            get { return m_OperationType; }
        }

        public TokenDiscardPolicy DiscardPolicy
        {
            get { return m_DiscardPolicy; }
        }

        public IdPolicy IdPolicy
        {
            get { return m_IdTokenPreceedsCurrentToken; }
        }

        public InputToken(Regex match, 
                          SemanticTokenType tokenType, 
                          OperationType operationType = OperationType.Operator, 
                          TokenDiscardPolicy discardPolicy = TokenDiscardPolicy.Keep,
                          IdPolicy idPolicity = IdPolicy.IdTokenPreceedsCurrentToken)
        {
            m_TokenType = tokenType;
            m_DiscardPolicy = discardPolicy;
            m_OperationType = operationType;
            m_Regex = match;
        }
    }
}
