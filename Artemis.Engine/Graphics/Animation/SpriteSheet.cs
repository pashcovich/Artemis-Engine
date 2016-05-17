#region Using Statements

using Artemis.Engine.Utilities.UriTree;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using System.Linq;

#endregion 

namespace Artemis.Engine.Graphics
{
    public class SpriteSheet
    {
        public struct Tile
        {
            /// <summary>
            /// The index of the texture associated with this rectangle in the list of
            /// LoadedTextures (not a property of TextureReference, but of the SpriteSheet
            /// a TextureReference belongs to).
            /// </summary>
            public readonly int TextureID;
            public readonly Rectangle Rectangle;
            public Tile(int id, Rectangle rectangle)
            {
                TextureID = id;
                Rectangle = rectangle;
            }
        }

        public const string GLOBAL_TILE_GROUP_NAME = "";

        /// <summary>
        /// The list of loaded textures to draw rectangles from in this sprite sheet.
        /// </summary>
        public List<Texture2D> LoadedTextures { get; private set; }

        /// <summary>
        /// The global group of tiles.
        /// </summary>
        private Dictionary<string, Tile> GlobalTileGroup;

        /// <summary>
        /// The dictionary of non-global tile groups.
        /// </summary>
        private Dictionary<string, Dictionary<string, Tile>> TileGroups;

        public SpriteSheet(List<Texture2D> texture, Dictionary<string, Dictionary<string, Tile>> tileGroups)
        {
            LoadedTextures = texture;
            if (tileGroups.ContainsKey(GLOBAL_TILE_GROUP_NAME))
            {
                GlobalTileGroup = tileGroups[GLOBAL_TILE_GROUP_NAME];
            }
            TileGroups = tileGroups.Where(v => v.Key != GLOBAL_TILE_GROUP_NAME).ToDictionary(v => v.Key, v => v.Value);
        }

        public Tile this[string key]
        {
            get
            {
                if (key.Contains(UriUtilities.URI_SEPARATOR))
                {
                    var parts = key.Split(UriUtilities.URI_SEPARATOR);
                    return TileGroups[parts[0]][parts[1]];
                }
                else
                {
                    return GlobalTileGroup[key];
                }
            }
        }
    }
}
