using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspileTest.Nodes;
using TranspileTest.Parser;

namespace TranspileTest
{
    public enum ScriptVariableType
    {
        Unknown = 0,
        Integer = 1,
        Decimal = 2,
        Mask = 3,
        Flag = 4,
        String = 5
    }
    
    public enum ScriptVariableScope
    {
        Unknown = 0,
        Global = 1,
        Local = 2
    }
    
    public class ScriptVariableTable
    {
        public Dictionary<Guid, Int32> IntegerDictionary = new Dictionary<Guid, int>();
        public Dictionary<Guid, Decimal> DecimalDictionary = new Dictionary<Guid, Decimal>();

        public Dictionary<Guid, bool> FlagDictionary = new Dictionary<Guid, bool>();
        public Dictionary<Guid, ulong> MaskDictionary = new Dictionary<Guid, ulong>();
        public Dictionary<Guid, String> StringDictionary = new Dictionary<Guid, String>();
        
        // Various Helpful Lookups
        public Dictionary<Guid, String> VariableNamesById = new Dictionary<Guid, string>();
        public Dictionary<Guid, ScriptVariableType> VariableTypeById = new Dictionary<Guid, ScriptVariableType>();
        public Dictionary<string, Guid> VariableIdByName = new Dictionary<string, Guid>();

        public bool HasVariable(string variableName)
        {
            return VariableIdByName.ContainsKey(variableName);
        }

        public void SetVariable(string variableName, string value)
        {
            var id = VariableIdByName[variableName];
            var varType = VariableTypeById[id];
            switch(varType)
            {
                case ScriptVariableType.Flag:
                    {
                        FlagDictionary[id] = bool.Parse(value);
                        break;
                    }
                case ScriptVariableType.Integer:
                    {
                        IntegerDictionary[id] = int.Parse(value);
                        break;
                    }
                default:
                    {
                        throw new Exception("Unsupported variable type:" + varType);
                        break;
                    }
            }
        }

        internal int GetValueInt(Guid guid)
        {
            return IntegerDictionary[guid];
        }

        public bool GetValueBool(string v)
        {
            var id = VariableIdByName[v];
            return FlagDictionary[id];
        }

        public void AddVariable(Guid id, ScriptVariableType variableType, int value, string name)
        {
            IntegerDictionary.Add(id, value);
            VariableNamesById.Add(id, name);
            VariableTypeById.Add(id, variableType);
            VariableIdByName.Add(name, id); 
        }

        public void AddVariable(Guid id, ScriptVariableType variableType, bool value, string name)
        {
            FlagDictionary.Add(id, value);
            VariableNamesById.Add(id, name);
            VariableTypeById.Add(id, variableType);
            VariableIdByName.Add(name, id);
        }

        internal Guid GetGuid(string tokenValue)
        {
            // TODO Need to differentiate between global and local
            return VariableIdByName[tokenValue];

            //throw new NotImplementedException();
        }
    }
}
