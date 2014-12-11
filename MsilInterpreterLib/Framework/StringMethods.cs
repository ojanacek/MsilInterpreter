using System;
using MsilInterpreterLib.Components;

namespace MsilInterpreterLib.Framework
{
    internal sealed class StringJoin : DotMethod
    {
        public StringJoin(DotType type) : base("Join", type, true, typeof(string), null, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class StringFormat : DotMethod
    {
        public StringFormat(DotType type) : base("Format", type, true, typeof(string), null, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class StringSplit : DotMethod
    {
        public StringSplit(DotType type) : base("Split", type, false, typeof(string[]), new []{ typeof(char[]) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            var stringRef = interpreter.CurrentStackFrame.Arguments[0];
            var stringInstance = interpreter.GetFromHeap((Guid) stringRef);
            var separatorsRef = interpreter.CurrentStackFrame.Arguments[1];
            var separatorsInstance = interpreter.GetFromHeap((Guid) separatorsRef);
            var separators = separatorsInstance["Values"] as char[];
            var textToSplit = stringInstance["Value"].ToString();
            var parts = textToSplit.Split(separators);
            var resultRef = interpreter.CreateRefTypeArray(parts);
            interpreter.PushToStack(resultRef);
        }
    }
}
