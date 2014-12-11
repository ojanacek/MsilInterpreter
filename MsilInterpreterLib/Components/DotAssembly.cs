using System.Collections.Generic;

namespace MsilInterpreterLib.Components
{
    internal sealed class DotAssembly
    {
        private readonly string name;
        private readonly bool isFrameworkAssembly;

        public string Name { get { return name; } }
        public List<DotType> Types { get; set; }
        public bool IsFrameworkAssembly { get { return isFrameworkAssembly; } }

        public DotAssembly(string name, bool isFrameworkAssembly)
        {
            this.name = name;
            this.isFrameworkAssembly = isFrameworkAssembly;
            Types = new List<DotType>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}