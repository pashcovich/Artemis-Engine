
namespace Artemis.Engine.Graphics
{
    public abstract class RenderableObject : PhysicalObject
    {
        /// <summary>
        /// Whether or not the object has been rendered this cycle.
        /// </summary>
        public bool Rendered { get; internal set; }

        /// <summary>
        /// The components that specify how this object is to be rendered.
        /// </summary>
        public RenderComponents RenderComponents;

        /// <summary>
        /// An abstract method for rendering this object.
        /// </summary>
        public abstract void Render();
    }
}
