using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExpressionParser
{
    public class HostCall
    {
        private readonly string m_Name;
        private readonly Delegate m_Function;
        private readonly MethodInfo m_Method;
        private readonly object[] m_ParameterList;

        public Int32 Id { get; set; }

        public Int32 NumParameters { get; set; }

        public MethodInfo Method
        {
            get { return m_Method; }
        }

        public object[] ParameterList
        {
            get { return m_ParameterList; }
        }

        public string Name
        {
            get { return m_Name; }
        }

        public HostCall(string name, Delegate function)
        {
            m_Method = function.GetType().GetMethod("Invoke");
            if (m_Method.ReturnType != typeof(double))
                throw new ArgumentException("Required return type for delegate is double");

            m_Name = name;
            m_Function = function;
            var parameters = Method.GetParameters();
            m_ParameterList = new object[parameters.Length];
        }

        public Double Invoke()
        {
            return (Double)Method.Invoke(m_Function, ParameterList);
        }
    }
}
