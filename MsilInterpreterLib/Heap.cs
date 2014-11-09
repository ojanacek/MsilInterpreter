using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsilInterpreterLib
{
    internal sealed partial class Heap(int firstGenSize, int secondGenSize)
    {
        private readonly byte[] firstGen = new byte[firstGenSize];
        private readonly byte[] secondGen = new byte[secondGenSize];
        private readonly GarbageCollector gc = new GarbageCollector();
    }
}
