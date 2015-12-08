using System;

namespace Artemis.Engine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NamedMultiformAttribute : Attribute
    {
        public string Name { get; private set; }
        public NamedMultiformAttribute(string name)
        {
            Name = name;
        }
    }
}
