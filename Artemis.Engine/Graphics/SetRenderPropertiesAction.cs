
namespace Artemis.Engine.Graphics
{
    public sealed class SetRenderPropertiesAction : AbstractRenderOrderAction
    {
        private RenderPropertiesPacket packet;

        internal SetRenderPropertiesAction(RenderPropertiesPacket packet)
        {
            this.packet = packet;
        }

        public override void Perform(LayerManager manager)
        {
            ArtemisEngine.RenderPipeline.SetRenderProperties(packet);
        }
    }
}
