using System;

namespace MsilInterpreterLib
{
    internal sealed class HeapObject
    {
        private readonly object data;
        private readonly Type type;

        public object Data { get { return data; } }
        public Type Type { get { return type; } }

        public HeapObject(object data, Type type)
        {
            data = data;
            type = type;
        }
    }
}