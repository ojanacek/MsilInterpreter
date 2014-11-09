using System;
using System.Collections.Generic;

namespace MsilInterpreterLib
{
    internal sealed partial class Heap(int firstGenSize, int secondGenSize)
    {
        private readonly int firstGenMaxSize = firstGenSize;
        private readonly int secondGenMaxSize = secondGenSize;
        public Dictionary<int, HeapObject> FirstGen { get; } = new Dictionary<int, HeapObject>();
        public Dictionary<int, HeapObject> SecondGen { get; } = new Dictionary<int, HeapObject>();
        private readonly GarbageCollector gc = new GarbageCollector();

        public int Store(object data, Type type)
        {
            if (FirstGen.Count == firstGenMaxSize)
            {
                // gc
            }

            var heapObject = new HeapObject(data, type);
            FirstGen.Add(heapObject.GetHashCode(), heapObject);
            return heapObject.GetHashCode();
        }

        public HeapObject Get(int id)
        {
            HeapObject heapObject;
            if (!FirstGen.TryGetValue(id, out heapObject))
            {
                try { heapObject = SecondGen[id]; }
                catch (KeyNotFoundException)
                {
                    throw new KeyNotFoundException("Object with ID " + id + "have been already reclaimed or lost. This should not happen.");
                }
            }

            return heapObject;
        }
    }
}
