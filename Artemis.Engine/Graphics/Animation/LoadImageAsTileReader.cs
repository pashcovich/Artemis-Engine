#region Using Statements

using Artemis.Engine.Assets;
using Artemis.Engine.Utilities.UriTree;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

#endregion

namespace Artemis.Engine.Graphics.Animation
{
    public class LoadImageAsTileReader
    {
        #region Xml Constants
        
        // Xml Tags
        public const string ADJUSTMENTS = "Adjustments";
        public const string CROP_TILE   = "CropTile";

        // Xml Attribute Names
        public const string NAME        = "Name";
        public const string TILE_GROUP  = "TileGroup";
        public const string TOP_LEFT    = "TopLeft";
        public const string DIMENSIONS  = "Dimensions";

        // Xml Inner Text Regex
        public const string INT_REGEX              = @"(\s*[+-]?(?<!\.)\b[0-9]+\b(?!\.[0-9])\s*)";
        public const string SPACE_REGEX            = @"((\s*)?,?(\s*)?)";
        public const string DIMENSIONS_SPECE_REGEX = @"((\s*)?(x|X)?(\s*)?)";

        #endregion

        public XmlElement LoadImageTag { get; private set; }
        public Rectangle Tile { get; private set; }
        public Texture2D Texture { get; private set; }
        public string TileGroup { get; private set; }
        
        public LoadImageAsTileReader(XmlElement tag)
        {
            LoadImageTag = tag;
            Tile = new Rectangle();
            TileGroup = "";
        }

        public void read()
        {
            foreach (var node in LoadImageTag)
            {
                var element = node as XmlElement;

                if (element == null)
                {
                    continue;
                }

                ReadElementAttributes(element);

                if (element.HasChildNodes)
                {
                    ReadElementChildNodes(element);
                }
            }
        }

        private void ReadElementAttributes(XmlElement element)
        {
            Rectangle tempTile = new Rectangle(0, 0, Int32.MaxValue, Int32.MaxValue);
            foreach (XmlAttribute attrib in element.Attributes)
            {
                switch (attrib.Name)
                {
                    case NAME:
                        if (attrib.Value.Contains(UriUtilities.URI_SEPARATOR))
                        {
                            Texture = AssetLoader.Load<Texture2D>(attrib.Value);
                        }
                        else
                        {
                            Texture = AssetLoader.LoadUsingExtension(attrib.Value) as Texture2D;
                        }
                        break;

                    case TILE_GROUP:
                        TileGroup = element.Value;
                        if (String.IsNullOrEmpty(TileGroup))
                        {
                            TileGroup = "";
                        }
                        else
                        {
                            TileGroup += ".";
                        }
                        break;

                    case TOP_LEFT:
                        Match coords = Regex.Match(attrib.Value, INT_REGEX + SPACE_REGEX + INT_REGEX);

                        tempTile.X = Convert.ToInt32(coords.Groups[1].Value.Trim());
                        tempTile.Y = Convert.ToInt32(coords.Groups[5].Value.Trim());
                        break;

                    case DIMENSIONS:
                        Match dimensions = Regex.Match(attrib.Value, INT_REGEX + DIMENSIONS_SPECE_REGEX + INT_REGEX);
                        
                        tempTile.Width = Convert.ToInt32(dimensions.Groups[1].Value.Trim());
                        tempTile.Height = Convert.ToInt32(dimensions.Groups[6].Value.Trim());
                        break;

                    default:
                        break;
                }
            }
            Tile = tempTile;
        }

        private void ReadElementChildNodes(XmlElement parentNode)
        {
            foreach (var node in parentNode.ChildNodes)
            {
                var element = node as XmlElement;
                
                switch (element.Name)
                {
                    case ADJUSTMENTS:
                        ReadElementChildNodes(element);
                        break;

                    case CROP_TILE:
                        ReadElementAttributes(element);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
