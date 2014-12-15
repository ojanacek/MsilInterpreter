using System;
using MsilInterpreterLib.Components;

namespace MsilInterpreterLib.Framework
{
    internal sealed class EnvironmentGetNewLine : DotMethod
    {
        public EnvironmentGetNewLine(DotType declaringType) : base("get_NewLine", declaringType, true, typeof(string), null, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            ObjectInstance instance;
            var reference = interpreter.CreateObjectInstance(interpreter.LookUpType(typeof (string)), out instance);
            instance["Value"] = Environment.NewLine;
            interpreter.PushToStack(reference);
        }
    }
}