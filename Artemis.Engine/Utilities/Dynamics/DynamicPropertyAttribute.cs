#region Using Statements

using System;

#endregion

namespace Artemis.Engine.Utilities.Dynamics
{
    /// <summary>
    /// An attribute used to indicate when a field should be tracked as a
    /// DynamicField by an ArtemisObject's DynamicFieldContainer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DynamicPropertyAttribute : Attribute { }
}
