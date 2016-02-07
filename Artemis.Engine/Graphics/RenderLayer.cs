#region Using Statements

using Artemis.Engine.Utilities;
using Artemis.Engine.Utilities.UriTree;

using Microsoft.Xna.Framework.Graphics;

using System;

#endregion

namespace Artemis.Engine.Graphics
{
    public class RenderLayer : AbstractLayer
    {
        public RenderLayer(string fullName) : base(fullName) { }

        /// <summary>
        /// Render this layer and every sublayer as well.
        /// </summary>
        public override void Render()
        {
            foreach (var subLayer in Subnodes)
            {
                subLayer.Value.Render();
            }
            AllSprites.Render(this);
        }

        public override void RenderIRenderable(IRenderable item)
        {
            var position = item.Position;
            var components = item.RenderComponent;

            ArtemisEngine.RenderPipeline.Render(
                components.Texture,
                position,
                components.Source,
                components.Tint,
                (float)components.Rotation,
                components.Offset,
                components.Scale,
                components.SpriteEffects
                );
        }
    }
}
