using System;

namespace MsilInterpreterLib
{
    internal sealed class HeapObject(object data, Type type)
    {
        public object Data { get; } = data;
        public Type Type { get; } = type;
    }
}