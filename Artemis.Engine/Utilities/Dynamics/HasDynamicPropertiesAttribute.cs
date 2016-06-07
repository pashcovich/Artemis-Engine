#region Using Statements

using System;

#endregion

namespace Artemis.Engine.Utilities.Dynamics
{
    /// <summary>
    /// An attribute representing whether or not a class has DynamicProperties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class HasDynamicPropertiesAttribute : Attribute
    {

        /// <summary>
        /// The array of the names of the DynamicProperties.
        /// </summary>
        public string[] DynamicPropertyNames { get; private set; }

        /// <summary>
        /// Whether or not the supplied list of dynamic properties is complete (i.e.
        /// accounts for all dynamic properties in base classes as well).
        /// </summary>
        public bool Complete { get; private set; }

        public HasDynamicPropertiesAttribute(string[] propNames, bool complete = false)
        {
            if (propNames.Length == 0)
            {
                throw new DynamicPropertyException(
                    "HasDynamicProperties attribute must be supplied more than 0 property names.");
            }
            DynamicPropertyNames = propNames;
            Complete = complete;
        }
    }
}
