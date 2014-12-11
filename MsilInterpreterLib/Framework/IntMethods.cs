using System;
using MsilInterpreterLib.Components;

namespace MsilInterpreterLib.Framework
{
    internal sealed class Int32Parse : DotMethod
    {
        public Int32Parse(DotType declaringType) : base("Parse", declaringType, true, typeof(int), new[] { typeof(string) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            var argument = interpreter.CurrentStackFrame.Arguments[0];
            var value = argument;
            if (argument is Guid)
                value = interpreter.GetFromHeap((Guid) argument)["Value"];

            interpreter.PushToStack(int.Parse(value.ToString()));
        }
    }
}