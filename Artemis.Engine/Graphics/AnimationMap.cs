#region Using Statements

using Artemis.Engine;

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Artemis.Engine.Graphics
{
    public class AnimationMap : UpdatableObject
    {

        /// <summary>
        /// Dictionary of all states in this animation map.
        /// </summary>
        public Dictionary<string, AnimationState> States { get; private set; }
        
        /// <summary>
        /// The name of the current state in the States dictionary.
        /// </summary>
        public string CurrentState { get; private set; }

        public SpriteSheet SpriteSheet { get; private set; }

        public AnimationMap(Dictionary<string, AnimationState> states, SpriteSheet spriteSheet, string initState)
            : base()
        {
            States = states;
            SpriteSheet = spriteSheet;
            CurrentState = initState;
        }

        public AnimationMap(List<AnimationState> states, SpriteSheet spriteSheet, string initState)
            : this(states.ToDictionary(s => s.Name, s => s), spriteSheet, initState) { }
        
        public void SetState(string stateName)
        {
            if (!States.ContainsKey(stateName))
            {
                throw new AnimationStateException(String.Format("Invalid state name: '{0}'.", stateName));
            }
            CurrentState = stateName;
        }

        internal override void AuxiliaryUpdate()
        {
            base.AuxiliaryUpdate();

            States[CurrentState].Update(this);
        }

    }
}
