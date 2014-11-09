using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsilInterpreterLib
{
    internal partial class Heap
    {
        /// <summary>
        /// Nested GC in the Heap class can access its inner structures.
        /// </summary>
        private class GarbageCollector
        {

        }
    }
}
