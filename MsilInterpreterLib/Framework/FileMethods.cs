using System;
using System.IO;
using MsilInterpreterLib.Components;

namespace MsilInterpreterLib.Framework
{
    internal sealed class FileReadAllLines : DotMethod
    {
        public FileReadAllLines(DotType type) : base("ReadAllLines", type, true, typeof(string[]), new []{ typeof(string) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            var pathObjRef = interpreter.CurrentStackFrame.Arguments[0];
            var pathObject = interpreter.GetFromHeap((Guid)pathObjRef);

            var path = pathObject["Value"].ToString();
            var lines = File.ReadAllLines(path);

            var linesReference = interpreter.CreateRefTypeArray(lines);
            interpreter.PushToStack(linesReference);
        }
    }

    internal sealed class FileWriteAllText : DotMethod
    {
        public FileWriteAllText(DotType type) : base("WriteAllText", type, true, typeof(void), new[] { typeof(string), typeof(string) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            var pathObjRef = interpreter.CurrentStackFrame.Arguments[0];
            var textObjRef = interpreter.CurrentStackFrame.Arguments[1];
            var pathObject = interpreter.GetFromHeap((Guid)pathObjRef);
            var textObject = interpreter.GetFromHeap((Guid)textObjRef);

            var path = pathObject["Value"].ToString();
            var text = textObject["Value"].ToString();
            File.WriteAllText(path, text);
        }
    }
}