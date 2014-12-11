using MsilInterpreterLib.Msil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MsilInterpreterLib.Components
{
    internal class DotConstructor : DotMethodBase
    {
        public DotConstructor(DotType declaringType, bool isStatic, Type[] parametersTypes, IEnumerable<ILInstruction> body) : base(declaringType, isStatic, parametersTypes, body)
        {
            
        }

        public Guid Invoke(Interpreter interpreter)
        {
            ObjectInstance instance;
            return interpreter.CreateObjectInstance(DeclaringType, out instance);
        }

        public override string ToString()
        {
            return string.Format("{0} .ctor({1})", DeclaringType, string.Join(", ", ParametersTypes.Select(t => t.Name)));
        }
    }
}
