using System.Collections.Generic;
using MsilInterpreterLib.Components;

namespace MsilInterpreterLib
{
    internal sealed class StackFrame
    {
        private readonly DotMethodBase caller;
        private readonly DotMethodBase currentMethod;
        private readonly Stack<object> stack = new Stack<object>();
        private readonly object[] locals = new object[20];

        public DotMethodBase Caller { get { return caller; } }
        public DotMethodBase CurrentMethod { get { return currentMethod; } }
        public Stack<object> Stack { get { return stack; } }
        public object[] Locals { get { return locals; } }

        public List<object> Arguments { get; set; }

        public StackFrame(DotMethodBase caller, DotMethodBase callee)
        {
            this.caller = caller;
            currentMethod = callee;
        }
    }
}