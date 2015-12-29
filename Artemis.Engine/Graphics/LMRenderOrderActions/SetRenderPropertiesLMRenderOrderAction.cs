
namespace Artemis.Engine.Graphics.LMRenderOrderActions
{
    public sealed class SetRenderPropertiesLMRenderOrderAction : AbstractLMRenderOrderAction
    {
        private RenderPropertiesPacket packet;

        internal SetRenderPropertiesLMRenderOrderAction(RenderPropertiesPacket packet)
        {
            this.packet = packet;
        }

        public override void Perform(LayerManager manager)
        {
            ArtemisEngine.RenderPipeline.SetRenderProperties(packet);
        }
    }
}
