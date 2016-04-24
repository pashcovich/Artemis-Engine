#region Using Statements

using Artemis.Engine.Graphics.AnimationStepActions;

using System;
using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Graphics
{
    public class AnimationState
    {
        /// <summary>
        /// The name of this animation state.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The name of the current frame being rendered.
        /// </summary>
        public string CurrentFrame { get; internal set; }

        /// <summary>
        /// The LoopType of this animation state.
        /// </summary>
        public AnimationStateLoopType LoopType { get; private set; }

        /// <summary>
        /// The number of times the animation state should loop before `Finished` is set to true.
        /// If this is null, the animation will loop indefinitely.
        /// </summary>
        public int? LoopCount { get; private set; }

        /// <summary>
        /// Whether or not the animation state is finished playing.
        /// </summary>
        public bool Finished { get; private set; }

        /// <summary>
        /// The number of step actions in this animation state.
        /// </summary>
        public int NumStepActions { get { return stepActions.Count; } }

        /// <summary>
        /// The number of times we've looped so far. This number is always less than
        /// the LoopCount property.
        /// </summary>
        public int TimesLooped { get; private set; }

        private int direction = 1;

        private int currentStepAction = 0;

        private int jumpPoint = -1;

        private List<AbstractAnimationStepAction> stepActions;

        public AnimationState(string name, List<AbstractAnimationStepAction> stepActions, AnimationStateLoopType loopType)
        {
            Name = name;
            LoopType = loopType;
            
            this.stepActions = stepActions;
        }

        // CheckForLoop is a temporary name.
        private void CheckForLoop()
        {
            if (direction == 1)
            {
                LoopInDirection(NumStepActions, 0, NumStepActions - 2);
            }
            else if (direction == -1)
            {
                LoopInDirection(0, NumStepActions - 1, 1);
            }
        }

        // Also a temporary name.
        private void LoopInDirection(
            int terminalStepAction, int nextStepAction_Cycle, int nextStepAction_Reverse)
        {
            if (currentStepAction == terminalStepAction)
            {
                TimesLooped++;
                if (LoopCount.HasValue && TimesLooped == LoopCount.Value)
                {
                    Finished = true;
                }
                else
                {
                    switch (LoopType)
                    {
                        case AnimationStateLoopType.Cycle:
                            currentStepAction = nextStepAction_Cycle;
                            break;
                        case AnimationStateLoopType.Reverse:
                            currentStepAction = nextStepAction_Reverse;
                            break;
                        default:
                            throw new AnimationStateLoopTypeException(
                                String.Format(
                                    "Invalid LoopType encountered: '{0}'.", LoopType
                                    )
                                );
                    }
                }
            }
        }

        internal void Reverse()
        {
            direction *= -1;
        }

        internal void JumpTo(int position)
        {
            if (!(position >= 0 && position < NumStepActions))
            {
                throw new AnimationStateException(
                    String.Format(
                        "Invalid jump point '{0}'. Value must be between 0 and the " +
                        "number of step actions - 1 (inclusive).", position
                        )
                    );
            }
            jumpPoint = position;
        }

        public void Update(AnimationMap map)
        {
            stepActions[currentStepAction].Perform(this, map);
            if (stepActions[currentStepAction].Finished)
            {
                if (jumpPoint != -1)
                {
                    currentStepAction = jumpPoint;
                    jumpPoint = -1;
                }
                else
                {
                    currentStepAction += direction;
                }
                CheckForLoop();
            }
        }
    }
}
