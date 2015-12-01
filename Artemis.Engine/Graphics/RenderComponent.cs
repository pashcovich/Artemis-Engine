using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Artemis.Engine.Graphics
{
    public class RenderComponent
    {
        public Texture2D Texture { get; protected set; }

        public RenderComponent(string textureName)
        {
            Texture = AssetLoader.Load<Texture2D>(textureName);
        }

        public RenderComponent(Texture2D texture)
        {
            Texture = texture;
        }
    }
}
