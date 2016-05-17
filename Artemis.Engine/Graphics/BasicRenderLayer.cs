
namespace Artemis.Engine.Graphics
{
    /// <summary>
    /// The most basic, non-abstract layer.
    /// </summary>
    public class BasicRenderLayer : AbstractOrderableRenderLayer
    {
        public BasicRenderLayer(string fullName)
            : base(fullName) { }

        protected override RenderableHandler GetRenderableHandler()
        {
            return obj => obj.InternalRender(SeenRenderables);
        }
    }
}
