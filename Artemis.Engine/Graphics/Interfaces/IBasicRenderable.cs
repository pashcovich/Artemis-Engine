using Microsoft.Xna.Framework;

namespace Artemis.Engine.Graphics
{
    public interface IBasicRenderable : INullRenderable
    {
        /// <summary>
        /// The render current render properties of this object.
        /// </summary>
        RenderComponent CurrentRenderComponent { get; set; }

        /// <summary>
        /// The position of this object. 
        /// </summary>
        Vector2 Position { get; set; }
    }
}
