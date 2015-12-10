#region Using Statements

using System;

#endregion

namespace Artemis.Engine
{
    /// <summary>
    /// An attribute indicating that an ArtemisObject should be force-updated every
    /// frame by the GlobalUpdater.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ForceUpdateAttribute : Attribute { }
}
