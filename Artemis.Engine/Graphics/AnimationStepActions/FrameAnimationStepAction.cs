
namespace Artemis.Engine.Graphics.AnimationStepActions
{
    public class FrameAnimationStepAction : AbstractAnimationStepAction
    {

        public string FrameName { get; private set; }

        public FrameAnimationStepAction(string frameName)
        {
            FrameName = frameName;
        }

        public override void Perform(AnimationState state, AnimationMap map)
        {
            state.CurrentFrame = FrameName;
            Finished = true;
        }
    }
}
