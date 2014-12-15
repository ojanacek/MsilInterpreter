using System;
using System.Collections.Generic;
using System.Linq;
using MsilInterpreterLib.Components;

namespace MsilInterpreterLib.Framework
{
    internal sealed class StringConcat2 : StringConcat
    {
        public StringConcat2(DotType type) : base(type, new[] { typeof(string), typeof(string) })
        {
        }
    }

    internal sealed class StringConcat3 : StringConcat
    {
        public StringConcat3(DotType type) : base(type, new []{ typeof(string), typeof(string), typeof(string) })
        {
        }
    }

    internal abstract class StringConcat : DotMethod
    {
        protected StringConcat(DotType type, Type[] parametersTypes) : base("Concat", type, true, typeof(string), parametersTypes, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            var result = "";
            for (int i = 0; i < ParametersCount; i++)
            {
                var strRef = interpreter.CurrentStackFrame.Arguments[i];
                var strValue = interpreter.GetFromHeap((Guid)strRef)["Value"];
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
        public StringFormat(DotType type) : base("Format", type, true, typeof(string), new []{ typeof(string), typeof(string) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            var formatRef = interpreter.CurrentStackFrame.Arguments[0];
            var format = interpreter.GetFromHeap((Guid) formatRef)["Value"] as string;
            var valueRef = interpreter.CurrentStackFrame.Arguments[1];
            var value = interpreter.GetFromHeap((Guid) valueRef)["Value"] as string; // simplified scenario, just one string instead of many objects

            var result = string.Format(format, value);
            ObjectInstance instance;
            var reference = interpreter.CreateObjectInstance(interpreter.LookUpType(typeof (string)), out instance);
            instance["Value"] = result;
            interpreter.PushToStack(reference);
        }
    }

    internal sealed class StringJoin : DotMethod
    {
        public StringJoin(DotType type) : base("Join", type, true, typeof(string), new []{ typeof(string), typeof(IEnumerable<object>) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            var separatorRef = interpreter.CurrentStackFrame.Arguments[0];
            var separator = interpreter.GetFromHeap((Guid) separatorRef)["Value"] as string;
            var valuesRef = interpreter.CurrentStackFrame.Arguments[1];
            var result = "";

            var listValues = interpreter.GetFromHeap((Guid) valuesRef)["Values"] as List<object>;
            if (listValues != null)
            {
                if (listValues[0] is Guid)
                {
                    listValues = listValues.Select(v => interpreter.GetFromHeap((Guid)v)["Value"]).ToList();
                }

                result = string.Join(separator, listValues);
            }
            else
            {
                var arrayValues = interpreter.GetFromHeap((Guid)valuesRef)["Values"] as object[];
                if (arrayValues != null)
                {
                    result = string.Join(separator, arrayValues);
                }
                else
                {
                    var arrayRefs = interpreter.GetFromHeap((Guid)valuesRef)["Values"] as Guid[];
                    if (arrayRefs != null)
                    {
                        arrayValues = arrayRefs.Where(v => v != Guid.Empty).Select(v => interpreter.GetFromHeap(v)["Value"]).ToArray();
                        result = string.Join(separator, arrayValues);
                    }
                }
            }
            
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
