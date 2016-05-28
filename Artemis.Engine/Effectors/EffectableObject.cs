#region Using Statements

using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Effectors
{
    public class EffectableObject : TimeableObject
    {
        /// <summary>
        /// The dictionary of effectors applied to this object that have names.
        /// </summary>
        internal readonly Dictionary<string, IEffector> NamedEffectors
            = new Dictionary<string, IEffector>();

        /// <summary>
        /// The list of effectors applied to this object that don't have names.
        /// </summary>
        internal readonly List<IEffector> AnonymousEffectors = new List<IEffector>();

        public EffectableObject() : base() { }

        /// <summary>
        /// Add an effector to this object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="effector"></param>
        public void AddEffector(IEffector effector)
        {
            if (effector.Anonymous)
            {
                AnonymousEffectors.Add(effector);
            }
            else
            {
                NamedEffectors.Add(effector.EffectorName, effector);
            }
            effector.InternalInitialize(this);
        }

        internal override void InternalUpdate()
        {
            base.InternalUpdate();

            if (!IsPaused)
            {
                foreach (var kvp in NamedEffectors)
                {
                    kvp.Value.UpdateEffector(this);
                }

                foreach (var effector in AnonymousEffectors)
                {
                    effector.UpdateEffector(this);
                }
            }
        }
    }
}
