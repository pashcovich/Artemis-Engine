
namespace Artemis.Engine.Graphics.AnimationStepActions
{
    public class ReverseAnimationStepAction : AbstractAnimationStepAction
    {
        public override void Perform(AnimationState state, AnimationMap map)
        {
            state.Reverse();
            Finished = true;
        }
    }
}
