
namespace Artemis.Engine.Graphics
{
    public sealed class RenderLayerAction : AbstractRenderOrderAction
    {
        private string layerName;

        internal RenderLayerAction(string name)
        {
            layerName = name;
        }

        public override void Perform(LayerManager manager)
        {
            manager.GetObservedNode(layerName).Render();
        }
    }
}
