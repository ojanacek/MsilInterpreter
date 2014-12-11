using System;
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
            listInstance["Values"] = new object[capacity];
            listInstance["Capacity"] = capacity;
            listInstance["Count"] = 0;
        }
    }

    internal sealed class ListAdd : DotMethod
    {
        public ListAdd(DotType declaringType) : base("Add", declaringType, false, typeof(void), new []{ typeof(object) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class ListAddRange : DotMethod
    {
        public ListAddRange(DotType declaringType) : base("AddRange", declaringType, false, typeof(void), new[] { typeof(object) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class ListGetRange : DotMethod
    {
        public ListGetRange(DotType declaringType) : base("GetRange", declaringType, false, typeof(void), new[] { typeof(object) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
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
            throw new NotImplementedException();
        }
    }

    internal sealed class ListIndexer : DotMethod
    {
        public ListIndexer(DotType declaringType) : base("[]", declaringType, false, typeof(object), new[] { typeof(int) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class ListInsert : DotMethod
    {
        public ListInsert(DotType declaringType) : base("Insert", declaringType, false, typeof(void), new[] { typeof(int), typeof(object) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class ListRemoveAt : DotMethod
    {
        public ListRemoveAt(DotType declaringType) : base("RemoveAt", declaringType, false, typeof(void), new[] { typeof(int) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class ListRemoveRange : DotMethod
    {
        public ListRemoveRange(DotType declaringType) : base("RemoveRange", declaringType, false, typeof(void), new[] { typeof(object) }, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            throw new NotImplementedException();
        }
    }
}