#region Using Statements

using System;

#endregion

namespace Artemis.Engine.Utilities.Partial
{
    /// <summary>
    /// An attribute that indicates when an object that inherits from PartialEngineAdapter
    /// is running in a partial engine environment. For a definition of "partial engine
    /// environment", see the PartialEngineAdapter summary.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PartialEngineAttribute : Attribute
    {
        public PartialEngineAttribute() : base() { }
    }
}
