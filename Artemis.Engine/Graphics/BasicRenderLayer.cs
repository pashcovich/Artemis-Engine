
namespace Artemis.Engine.Graphics
{
    /// <summary>
    /// The most basic, non-abstract layer.
    /// </summary>
    public class BasicRenderLayer : AbstractOrderableRenderLayer
    {
        public BasicRenderLayer(string fullName)
            : base(fullName) { }

        protected override void PreRender()
        {
            // Reset the RenderTarget if the resolution has changed.
            if (ArtemisEngine.DisplayManager.ResolutionChanged)
            {
                LayerTarget.Dispose();

                LayerTarget = ArtemisEngine.RenderPipeline.CreateRenderTarget();
            }
        }

        protected override RenderableHandler GetRenderableHandler()
        {
            return obj => obj.InternalRender(SeenRenderables);
        }
    }
}
