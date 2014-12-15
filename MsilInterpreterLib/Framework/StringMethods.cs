﻿using System;
using System.Collections.Generic;
using System.Linq;
using MsilInterpreterLib.Components;

namespace MsilInterpreterLib.Framework
{
    internal sealed class StringConcat3 : DotMethod
    {
        public StringConcat3(DotType type) : base("Concat", type, true, typeof(string), new []{ typeof(string), typeof(string), typeof(string) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            var result = "";
            for (int i = 0; i < 3; i++)
            {
                var strRef = interpreter.CurrentStackFrame.Arguments[i];
                var strValue = interpreter.GetFromHeap((Guid) strRef)["Value"];
                var str = strValue == null ? "" : strValue.ToString();
                result += str;
            }
            ObjectInstance instance;
            var resultRef = interpreter.CreateObjectInstance(interpreter.LookUpType(typeof(string)), out instance);
            instance["Value"] = result;
            interpreter.PushToStack(resultRef);
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

    internal sealed class StringJoin : DotMethod
    {
        public StringJoin(DotType type) : base("Join", type, true, typeof(string), new []{ typeof(string), typeof(List<object>) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            var separatorRef = interpreter.CurrentStackFrame.Arguments[0];
            var separator = interpreter.GetFromHeap((Guid) separatorRef)["Value"] as string;
            var valuesRef = interpreter.CurrentStackFrame.Arguments[1];
            var values = interpreter.GetFromHeap((Guid)valuesRef)["Values"] as List<object>;

            if (values[0] is Guid)
            {
                values = values.Select(v => interpreter.GetFromHeap((Guid) v)["Value"]).ToList();
            }

            var result = string.Join(separator, values);
            ObjectInstance instance;
            var resultRef = interpreter.CreateObjectInstance(interpreter.LookUpType(typeof(string)), out instance);
            instance["Value"] = result;
            interpreter.PushToStack(resultRef);
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
            var textToSplit = interpreter.GetFromHeap((Guid)stringRef)["Value"] as string;
            var separatorsRef = interpreter.CurrentStackFrame.Arguments[1];
            var separators = interpreter.GetFromHeap((Guid)separatorsRef)["Values"] as char[];

            var parts = textToSplit.Split(separators);
            var resultRef = interpreter.CreateRefTypeArray(parts);
            interpreter.PushToStack(resultRef);
        }
    }
}
