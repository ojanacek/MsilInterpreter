using System;
using System.Collections.Generic;
using MsilInterpreterLib.Components;

namespace MsilInterpreterLib.Framework
{
    internal sealed class ListCapacityCtor : DotConstructor
    {
        public ListCapacityCtor(DotType declaringType) : base(declaringType, false, new []{ typeof(int) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            var instanceRef = (Guid)interpreter.CurrentStackFrame.Arguments[0];
            var capacity = (int)interpreter.CurrentStackFrame.Arguments[1];
            var listInstance = interpreter.GetFromHeap(instanceRef);
            listInstance["Values"] = new List<object>(capacity);
        }
    }

    internal sealed class ListAddRange : DotMethod
    {
        public ListAddRange(DotType declaringType) : base("AddRange", declaringType, false, typeof(void), new[] { typeof(IEnumerable<object>) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            /*var instanceRef = (Guid)interpreter.CurrentStackFrame.Arguments[0];
            var listInstance = interpreter.GetFromHeap(instanceRef);
            var index = (int)interpreter.CurrentStackFrame.Arguments[1];
            var valueToInsert = interpreter.CurrentStackFrame.Arguments[2];
            var list = listInstance["Values"] as List<object>;
            list.Insert(index, valueToInsert);*/
            throw new NotImplementedException();
        }
    }

    internal sealed class ListGetCount : DotMethod
    {
        public ListGetCount(DotType declaringType) : base("get_Count", declaringType, false, typeof(object), null, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            var instanceRef = (Guid)interpreter.CurrentStackFrame.Arguments[0];
            var listInstance = interpreter.GetFromHeap(instanceRef);
            var list = listInstance["Values"] as List<object>;
            interpreter.PushToStack(list.Count);
        }
    }

    internal sealed class ListGetItem : DotMethod
    {
        public ListGetItem(DotType declaringType) : base("get_Item", declaringType, false, typeof(object), new[] { typeof(int) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            var instanceRef = (Guid)interpreter.CurrentStackFrame.Arguments[0];
            var listInstance = interpreter.GetFromHeap(instanceRef);
            int index = (int) interpreter.CurrentStackFrame.Arguments[1];
            var list = listInstance["Values"] as List<object>;
            interpreter.PushToStack(list[index]);
        }
    }

    internal sealed class ListGetRange : DotMethod
    {
        public ListGetRange(DotType declaringType) : base("GetRange", declaringType, false, typeof(List<object>), new[] { typeof(int), typeof(int) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            /*var instanceRef = (Guid)interpreter.CurrentStackFrame.Arguments[0];
            var listInstance = interpreter.GetFromHeap(instanceRef);
            var index = (int)interpreter.CurrentStackFrame.Arguments[1];
            var valueToInsert = interpreter.CurrentStackFrame.Arguments[2];
            var list = listInstance["Values"] as List<object>;
            list.Insert(index, valueToInsert);*/
            throw new NotImplementedException();
        }
    }

    internal sealed class ListIndexOf : DotMethod
    {
        public ListIndexOf(DotType declaringType) : base("IndexOf", declaringType, false, typeof(int), new[] { typeof(object) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            var instanceRef = (Guid)interpreter.CurrentStackFrame.Arguments[0];
            var listInstance = interpreter.GetFromHeap(instanceRef);
            var searchFor = interpreter.CurrentStackFrame.Arguments[1];
            var list = listInstance["Values"] as List<object>;
            int index = list.IndexOf(searchFor);
            interpreter.PushToStack(index);
        }
    }

    internal sealed class ListInsert : DotMethod
    {
        public ListInsert(DotType declaringType) : base("Insert", declaringType, false, typeof(void), new[] { typeof(int), typeof(object) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            var instanceRef = (Guid)interpreter.CurrentStackFrame.Arguments[0];
            var listInstance = interpreter.GetFromHeap(instanceRef);
            var index = (int)interpreter.CurrentStackFrame.Arguments[1];
            var valueToInsert = interpreter.CurrentStackFrame.Arguments[2];
            var list = listInstance["Values"] as List<object>;
            list.Insert(index, valueToInsert);
        }
    }

    internal sealed class ListRemoveAt : DotMethod
    {
        public ListRemoveAt(DotType declaringType) : base("RemoveAt", declaringType, false, typeof(void), new[] { typeof(int) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            var instanceRef = (Guid)interpreter.CurrentStackFrame.Arguments[0];
            var listInstance = interpreter.GetFromHeap(instanceRef);
            int index = (int)interpreter.CurrentStackFrame.Arguments[1];
            var list = listInstance["Values"] as List<object>;
            list.RemoveAt(index);
        }
    }

    internal sealed class ListRemoveRange : DotMethod
    {
        public ListRemoveRange(DotType declaringType) : base("RemoveRange", declaringType, false, typeof(void), new[] { typeof(int), typeof(int) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            var instanceRef = (Guid)interpreter.CurrentStackFrame.Arguments[0];
            var listInstance = interpreter.GetFromHeap(instanceRef);
            int index = (int)interpreter.CurrentStackFrame.Arguments[1];
            int count = (int)interpreter.CurrentStackFrame.Arguments[2];
            var list = listInstance["Values"] as List<object>;
            list.RemoveRange(index, count);
        }
    }
}