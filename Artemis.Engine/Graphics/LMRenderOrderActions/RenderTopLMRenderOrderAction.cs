
namespace Artemis.Engine.Graphics.LMRenderOrderActions
{
    public sealed class RenderTopLMRenderOrderAction : AbstractLMRenderOrderAction
    {
        private string layerName;

        internal RenderTopLMRenderOrderAction(string name)
        {
            layerName = name;
        }

        public override void Perform(LayerManager manager)
        {
            manager.GetObservedNode(layerName).RenderTop();
        }
    }
}
