#region Using Statements

using System;

#endregion

namespace Artemis.Engine
{
    /// <summary>
    /// An attribute indicating that a multiform uses its `Reconstruct` method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ReconstructMultiformAttribute : Attribute
    {
        public ReconstructMultiformAttribute() { }
    }
}
