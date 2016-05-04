
namespace Artemis.Engine.Graphics.AnimationStepActions
{
    public class JumpToAnimationStepAction : AbstractAnimationStepAction
    {

        public int JumpPoint { get; private set; }

        public JumpToAnimationStepAction(int jumpPoint)
        {
            JumpPoint = jumpPoint;
        }

        public override void Perform(AnimationState state, AnimationMap map)
        {
            state.JumpTo(JumpPoint);
        }
    }
}
