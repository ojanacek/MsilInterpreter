using System.Collections.Generic;

namespace MsilInterpreterLib.Components
{
    internal sealed class DotType
    {
        private readonly DotAssembly module;
        private readonly string name;

        public DotAssembly Module { get { return module; } }
        public string Name { get { return name; } }

        public List<DotField> Fields { get; set; }
        public List<DotConstructor> Constructors { get; set; }
        public List<DotMethod> Methods { get; set; }

        public DotType(string name, DotAssembly module)
        {
            this.name = name;
            this.module = module;
            Fields = new List<DotField>(0);
            Constructors = new List<DotConstructor>(0);
            Methods = new List<DotMethod>(0);
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Module, Name);
        }
    }
}