#region Using Statements

using System;

#endregion

namespace Artemis.Engine.Multiforms
{
    /// <summary>
    /// An attribute that represents the name to be given to instances of
    /// a multiform subclass.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NamedMultiformAttribute : Attribute
    {
        /// <summary>
        /// The name of the multiform instances.
        /// </summary>
        public string Name { get; private set; }

        public NamedMultiformAttribute(string name)
        {
            Name = name;
        }
    }
}
