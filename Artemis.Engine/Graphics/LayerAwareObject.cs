
namespace Artemis.Engine.Graphics
{
    public class LayerAwareObject : ResolutionRelativeObject
    {
        /// <summary>
        /// The Layer this object belongs to.
        /// </summary>
        public RenderLayer Layer { get; internal set; }


        public AbstractCamera Camera { get { return Layer.Camera; } }

        public LayerAwareObject() : base() { }
    }
}
