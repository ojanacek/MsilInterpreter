using MsilInterpreterLib.Msil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MsilInterpreterLib.Components
{
    internal abstract class DotMethodBase
    {
        private readonly DotType declaringType;
        private readonly bool isStatic;
        private readonly List<ILInstruction> body;
        private readonly Type[] parametersTypes;

        public DotType DeclaringType { get { return declaringType; } }
        public bool IsStatic { get { return isStatic; } }
        public ReadOnlyCollection<ILInstruction> Body { get { return body.AsReadOnly(); } }
        public Type[] ParametersTypes { get { return parametersTypes; } }

        protected DotMethodBase(DotType declaringType, bool isStatic, Type[] parametersTypes, IEnumerable<ILInstruction> body)
        {
            this.declaringType = declaringType;
            this.isStatic = isStatic;
            this.parametersTypes = parametersTypes ?? new Type[0];
            this.body = body == null ? new List<ILInstruction>() : new List<ILInstruction>(body);
        }

        public virtual void Execute(Interpreter interpreter)
        {
            interpreter.Execute(this);
        }
    }
}
