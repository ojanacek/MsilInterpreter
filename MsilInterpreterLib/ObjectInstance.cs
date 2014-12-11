using System;
using System.Linq;
using MsilInterpreterLib.Components;

namespace MsilInterpreterLib
{
    internal sealed class ObjectInstance
    {
        private readonly DotType typeHandler;
        private readonly object[] instanceFields;

        public DotType TypeHandler { get { return typeHandler; } }
        public object[] InstanceFields { get { return instanceFields; } }

        public object this[string fieldName]
        {
            get
            {
                var offset = typeHandler.Fields.FindIndex(f => f.Name == fieldName);
                if (offset == -1)
                    throw new IndexOutOfRangeException("Trying to access a field using invalid field name: " + fieldName + " in type: " + typeHandler);
                return instanceFields[offset];
            }
            set
            {
                var offset = typeHandler.Fields.FindIndex(f => f.Name == fieldName);
                if (offset == -1)
                    throw new IndexOutOfRangeException("Trying to access a field using invalid field name: " + fieldName + " in type: " + typeHandler);
                instanceFields[offset] = value;
            }
        }

        public ObjectInstance(DotType typeHandler)
        {
            this.typeHandler = typeHandler;
            instanceFields = new object[typeHandler.Fields.Count];
        }

        public override string ToString()
        {
            return String.Format("{0} - fields {1}", typeHandler, string.Join(";", instanceFields.Select(f => f == null ? "null" : f.ToString())));
        }
    }
}