using System;
using System.Collections.Generic;
using MsilInterpreterLib.Msil;
using System.Linq;

namespace MsilInterpreterLib.Components
{
    internal class DotMethod : DotMethodBase
    {
        private readonly string name;
        private readonly Type returnType;

        public string Name { get { return name; } }
        public Type ReturnType { get { return returnType; } }
        public bool IsAbstract { get; set; }
        public bool IsVirtual { get; set; }

        public DotMethod(string name, DotType declaringType, bool isStatic, Type returnType, Type[] parametersTypes, IEnumerable<ILInstruction> body) : base(declaringType, isStatic, parametersTypes, body)
        {
            this.name = name;
            this.returnType = returnType;
        }

        public override string ToString()
        {
            return string.Format("{0}::{1}({2})", DeclaringType, Name, string.Join(", ", ParametersTypes.Select(t => t.Name)));
        }
    }
}