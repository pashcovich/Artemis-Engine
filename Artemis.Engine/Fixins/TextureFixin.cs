#region Using Statements

using Artemis.Engine.Assets;

using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Artemis.Engine.Fixins
{
    public class TextureFixin : Fixin
    {

        public override FixinType FixinType { get { return FixinType.Render; } }

        /// <summary>
        /// The Texture to render.
        /// </summary>
        public Texture2D Texture;

        #region Constructors

        public TextureFixin(string name) : this(name, (Texture2D)null) { }

        public TextureFixin(string name, string textureName, bool treatAsAssetUri = true)
            : this(name, textureName, null, treatAsAssetUri) { }

        public TextureFixin(string name, Form form) : this(name, null, form) { }

        public TextureFixin(string name, Texture2D texture) : this(name, texture, null) { }

        public TextureFixin(string name, string textureName, Form form, bool treatAsAssetUri = true)
            : this(name, AssetLoader.Load<Texture2D>(textureName, treatAsAssetUri), form) { }

        #endregion

        public TextureFixin(string name, Texture2D texture, Form form)
            : base(name, form)
        {
            Texture = texture;
            RequiredRenderer += RenderTexture;
        }

        private void RenderTexture()
        {
            if (Texture != null)
            {
                var properties = Form.SpriteProperties;
                ArtemisEngine.RenderPipeline.Render(
                    Texture, Form.WorldPosition, properties);
            }
        }
    }
}
