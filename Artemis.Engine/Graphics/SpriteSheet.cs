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

        public const string GLOBAL_TILE_GROUP_NAME = "GLOBAL";

        public Texture2D Texture { get; private set; }

        private Dictionary<string, Rectangle> GlobalTileGroup;

        private Dictionary<string, Dictionary<string, Rectangle>> TileGroups;

        public SpriteSheet(Texture2D texture, Dictionary<string, Dictionary<string, Rectangle>> tileGroups)
        {
            Texture = texture;
            GlobalTileGroup = tileGroups[GLOBAL_TILE_GROUP_NAME];
            TileGroups = tileGroups.Where(v => v.Key != GLOBAL_TILE_GROUP_NAME).ToDictionary(v => v.Key, v => v.Value);
        }

        public Rectangle this[string key]
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
