
namespace Artemis.Engine.Graphics
{
    public class LayerAwareObject : ResolutionRelativeObject
    {
        public RenderLayer Layer { get; internal set; }

        public AbstractCamera Camera { get { return Layer.Camera; } }

        public LayerAwareObject() : base() { }
    }
}
