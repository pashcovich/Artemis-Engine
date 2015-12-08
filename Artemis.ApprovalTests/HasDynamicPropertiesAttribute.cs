using System;

namespace Artemis.Engine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class HasDynamicPropertiesAttribute : Attribute { }
}
