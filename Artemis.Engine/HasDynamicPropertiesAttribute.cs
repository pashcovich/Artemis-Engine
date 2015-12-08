using System;

namespace Artemis.Engine
{
    /// <summary>
    /// An attribute representing whether or not a class has DynamicProperties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class HasDynamicPropertiesAttribute : Attribute 
    {

        /// <summary>
        /// The array of the names of the DynamicProperties.
        /// </summary>
        public string[] DynamicPropertyNames { get; private set; }

        /// <summary>
        /// Whether or not an array of DynamicProperties has been supplied.
        /// </summary>
        public bool HasDynamicPropertyList { get { return DynamicPropertyNames.Length > 0; } }

        public HasDynamicPropertiesAttribute(params string[] propNames)
        {
            DynamicPropertyNames = propNames;
        }
    }
}
