using System;
using System.Collections.Generic;

namespace MsilInterpreterLib
{
    internal sealed partial class Heap
    {
        private readonly int firstGenMaxSize;
        private readonly int secondGenMaxSize;
        private readonly Dictionary<Guid, ObjectInstance> firstGen;
        private readonly GarbageCollector gc = new GarbageCollector();

        public Heap(int firstGenSize, int secondGenSize)
        {
            firstGenMaxSize = firstGenSize;
            secondGenMaxSize = secondGenSize;
            firstGen = new Dictionary<Guid, ObjectInstance>();
        }
        
        public ObjectInstance Get(Guid address)
        {
            ObjectInstance objectInstance;
            if (!firstGen.TryGetValue(address, out objectInstance))
            {
                throw new KeyNotFoundException("Object with with address " + address + " have been already reclaimed or lost. This should not happen.");
            }

            return objectInstance;
        }

        public Guid Store(ObjectInstance instance)
        {
            if (firstGen.Count == firstGenMaxSize)
            {
                // gc
                throw new Exception("Full heap.");
            }

            var address = Guid.NewGuid();
            firstGen.Add(address, instance);
            return address;
        }
    }
}
