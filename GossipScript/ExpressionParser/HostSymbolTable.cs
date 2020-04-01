using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ExpressionParser
{
    public class HostSymbolTable : ICloneable
    {
        private readonly Dictionary<Int32, HostSymbol> m_SymbolTableById = new Dictionary<int, HostSymbol>();

        private readonly Dictionary<String, HostSymbol> m_SymbolTableByName = new Dictionary<string, HostSymbol>();

        private Int32 m_NextId;

        public void RegisterSymbol(string name, Double value)
        {
            lock (this)
            {
                // Check if symbol already exists
                if (m_SymbolTableByName.ContainsKey(name))
                {
                    var oldSymbol = m_SymbolTableByName[name];
                    oldSymbol.SymbolId = m_NextId;
                    oldSymbol.SymbolLabel = name;
                    oldSymbol.SymbolValue = value;
                }
                else
                {
                    var symbol = new HostSymbol();
                    m_NextId++;
                    symbol.SymbolId = m_NextId;
                    symbol.SymbolLabel = name;
                    symbol.SymbolValue = value;

                    m_SymbolTableById.Add(symbol.SymbolId, symbol);
                    m_SymbolTableByName.Add(symbol.SymbolLabel, symbol);
                }
            }
        }

        public HostSymbol GetSymbolById(Int32 id)
        {
            return m_SymbolTableById[id];
        }

        public HostSymbol GetSymbolByName(String name)
        {
            return m_SymbolTableByName[name];
        }

        internal void SetSymbol(HostSymbol symbol)
        {
            m_SymbolTableById.Add(symbol.SymbolId,symbol);
            m_SymbolTableByName.Add(symbol.SymbolLabel, symbol);
        }

        public object Clone()
        {
            var rv = new HostSymbolTable();
            foreach (var hostSymbol in m_SymbolTableById.Values)
            {
                rv.SetSymbol(hostSymbol);
            }
            return rv;
        }
    }
}
