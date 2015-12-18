using Microsoft.Xna.Framework;

namespace Artemis.Engine.Graphics
{
    interface IRenderable
    {
        Vector2 Position { get; set; }
        RenderComponent RenderComponent { get; set; }
    }
}
