
namespace Artemis.Engine.Graphics
{
    /// <summary>
    /// An interface representing an object that can be rendered to a layer.
    /// </summary>
    public interface IRenderable : INullRenderable
    {
        void Render();
    }
}
