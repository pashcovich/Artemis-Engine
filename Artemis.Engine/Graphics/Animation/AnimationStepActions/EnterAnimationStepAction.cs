
namespace Artemis.Engine.Graphics.AnimationStepActions
{
    public class EnterAnimationStepAction : AbstractAnimationStepAction
    {

        public string StateName { get; private set; }

        public EnterAnimationStepAction(string stateName)
        {
            StateName = stateName;
        }

        public override void Perform(AnimationState state, AnimationMap map)
        {
            map.SetState(StateName);
            Finished = true;
        }
    }
}
