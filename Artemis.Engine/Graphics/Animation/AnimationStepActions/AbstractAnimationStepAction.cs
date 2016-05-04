
namespace Artemis.Engine.Graphics.AnimationStepActions
{
    public abstract class AbstractAnimationStepAction
    {

        public bool Finished { get; protected set; }

        public abstract void Perform(AnimationState state, AnimationMap map);
    }
}
