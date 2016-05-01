
namespace Artemis.Engine.Graphics
{
    public abstract class RenderableObject : PhysicalObject
    {
        public RenderComponents RenderComponents;

        public abstract void Render();
    }
}
