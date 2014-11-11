using System;
using System.Collections.Generic;

namespace MsilInterpreterLib
{
    internal sealed partial class Heap
    {
        private readonly int firstGenMaxSize;
        private readonly int secondGenMaxSize;
        private readonly Dictionary<int, HeapObject> firstGen;
        private readonly Dictionary<int, HeapObject> secondGen;

        public Dictionary<int, HeapObject> FirstGen { get { return firstGen; } }
        public Dictionary<int, HeapObject> SecondGen { get { return secondGen; } }
        private readonly GarbageCollector gc = new GarbageCollector();

        public Heap(int firstGenSize, int secondGenSize)
        {
            firstGenMaxSize = firstGenSize;
            secondGenMaxSize = secondGenSize;
            firstGen = new Dictionary<int, HeapObject>();
            secondGen = new Dictionary<int, HeapObject>();
        }

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
