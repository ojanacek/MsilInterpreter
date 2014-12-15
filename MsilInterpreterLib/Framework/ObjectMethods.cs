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

    internal sealed class ObjectToString : DotMethod
    {
        public ObjectToString(DotType declaringType) : base("ToString", declaringType, false, typeof(string), null, null)
        {
        }

        public override void Execute(Interpreter interpreter)
        {
            var typeName = DeclaringType.ToString();
            ObjectInstance instance;
            var reference = interpreter.CreateObjectInstance(interpreter.LookUpType(typeof (string)), out instance);
            instance["Value"] = typeName;
            interpreter.PushToStack(reference);
        }
    }
}