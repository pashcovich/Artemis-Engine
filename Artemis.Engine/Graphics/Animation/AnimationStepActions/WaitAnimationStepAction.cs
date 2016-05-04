
namespace Artemis.Engine.Graphics.AnimationStepActions
{
    public class WaitAnimationStepAction : AbstractAnimationStepAction
    {

        public int framesWaited { get; private set; }

        public int WaitFor { get; private set; }

        public WaitAnimationStepAction(int waitFor)
        {
            WaitFor = waitFor;
            framesWaited = 0;
        }

        public override void Perform(AnimationState state, AnimationMap map)
        {
            if (framesWaited == WaitFor)
            {
                Finished = true;
            }
            else
            {
                framesWaited++;
            }
        }
    }
}
