using System;

namespace Artemis.Engine.Multiforms
{
    /// <summary>
    /// An attribute indicating any constraints on which multiforms a given 
    /// multiform can transition from.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class TransitionConstraintsAttribute : Attribute
    {
        public string[] AllowedFrom { get; private set; }
        public string[] NotAllowedFrom { get; private set; }

        public TransitionConstraintsAttribute(
            string[] AllowedFrom = null,
            string[] NotAllowedFrom = null
            )
        {
            this.AllowedFrom    = AllowedFrom;
            this.NotAllowedFrom = NotAllowedFrom;
        }
    }
}
