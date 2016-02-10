﻿#region Using Statements

using Artemis.Engine.Effectors;
using System.Collections.Generic;

#endregion

namespace Artemis.Engine
{
    /// <summary>
    /// An EffectableArtemisObject is an ArtemisObject to which Effectors can
    /// be added.
    /// </summary>
    public class EffectableArtemisObject : ArtemisObject
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

        public EffectableArtemisObject() : base() { }

        /// <summary>
        /// Add an effector to this object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="effector"></param>
        public void AddEffector<T>(Effector<T> effector)
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

        internal override void Update()
        {
            base.Update();

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