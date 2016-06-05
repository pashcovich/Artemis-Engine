#region Using Statements

using Artemis.Engine.Assets;

using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Artemis.Engine.Fixins
{
    public class TextureFixin : BasePositionalFixin
    {
        public override FixinType FixinType { get { return FixinType.Render; } }

        /// <summary>
        /// The Texture to render.
        /// </summary>
        public Texture2D Texture;

        /// <summary>
        /// The coordinate space from which to get the position of the texture.
        /// </summary>
        public CoordinateSpace CoordinateSpace;

        #region Constructors

        public TextureFixin(string name) : this(name, (Texture2D)null) { }

        public TextureFixin( string name
                           , string textureName
                           , bool treatAsAssetUri = true
                           , CoordinateSpace coordinateSpace = CoordinateSpace.TargetSpace )
            : this(name, textureName, null, treatAsAssetUri, coordinateSpace) { }

        public TextureFixin( string name
                           , PositionalForm form
                           , CoordinateSpace coordinateSpace = CoordinateSpace.TargetSpace )
            : this(name, null, form, coordinateSpace) { }

        public TextureFixin( string name
                           , Texture2D texture
                           , CoordinateSpace coordinateSpace = CoordinateSpace.TargetSpace ) 
            : this(name, texture, null, coordinateSpace) { }

        public TextureFixin( string name
                           , string textureName
                           , PositionalForm form
                           , bool treatAsAssetUri = true
                           , CoordinateSpace coordinateSpace = CoordinateSpace.TargetSpace )
            : this(name, AssetLoader.Load<Texture2D>(textureName, treatAsAssetUri), form, coordinateSpace) { }

        #endregion

        public TextureFixin( string name
                           , Texture2D texture
                           , PositionalForm form
                           , CoordinateSpace coordinateSpace = CoordinateSpace.TargetSpace )
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
                    Texture, Form.GetPosition(CoordinateSpace), properties);
            }
        }
    }
}
