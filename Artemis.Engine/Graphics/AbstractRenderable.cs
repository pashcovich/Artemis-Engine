
namespace Artemis.Engine.Graphics
{
    public abstract class AbstractRenderable
    {

        /// <summary>
        /// If a renderable is "Valid", it can be rendered. Otherwise, it's ignored.
        /// </summary>
        public bool Valid { get; set; }

        public abstract void Render();
    }
}
