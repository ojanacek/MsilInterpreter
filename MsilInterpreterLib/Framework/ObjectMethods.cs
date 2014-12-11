using MsilInterpreterLib.Components;

namespace MsilInterpreterLib.Framework
{
    internal sealed class ObjectCtor : DotConstructor
    {
        public ObjectCtor(DotType declaringType) : base(declaringType, false, null, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            /* do nothing on purpose */
        }
    }
}