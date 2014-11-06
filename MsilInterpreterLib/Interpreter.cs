using System;
using System.Reflection;

namespace MsilInterpreterLib
{
    public class Interpreter
    {
        public void Run(Action action)
        {            
            var parser = new ILParser();
            var instructions = parser.ParseILFromMethod(action.GetMethodInfo());            
        }
    }
}