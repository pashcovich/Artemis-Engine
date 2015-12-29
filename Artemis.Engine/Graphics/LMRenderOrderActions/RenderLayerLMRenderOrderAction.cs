
namespace Artemis.Engine.Graphics.LMRenderOrderActions
{
    public sealed class RenderLayerLMRenderOrderAction : AbstractLMRenderOrderAction
    {
        private string layerName;

        internal RenderLayerLMRenderOrderAction(string name)
        {
            layerName = name;
        }

        public override void Perform(LayerManager manager)
        {
            manager.GetObservedNode(layerName).Render();
        }
    }
}
