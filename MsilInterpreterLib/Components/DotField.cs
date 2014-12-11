namespace MsilInterpreterLib.Components
{
    internal sealed class DotField
    {
        private readonly string name;
        private readonly DotType declaringType;

        public string Name { get { return name; } }
        public DotType DeclaringType { get { return declaringType; } }

        public DotField(string name, DotType declaringType)
        {
            this.name = name;
            this.declaringType = declaringType;
        }

        public override string ToString()
        {
            return string.Format("{0}::{1}", DeclaringType, Name);
        }
    }
}