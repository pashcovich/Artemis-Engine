#region Using Statements

using Artemis.Engine.Assets;
using Artemis.Engine.Utilities.UriTree;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

#endregion

namespace Artemis.Engine.Graphics.Animation
{
    public class LoadSheetReader
    {
        #region Xml Constants

        // Xml Tags
        public const string ADJUSTMENTS = "Adjustments";
        public const string CROP_TILE   = "CropTile";
        public const string TILE_GROUP  = "TileGroup";
        public const string TILE        = "Tile";

        // Xml Attribute Names
        public const string NAME        = "Name";
        public const string TILE_GROUPS = "TileGroups";
        public const string TOP_LEFT    = "TopLeft";
        public const string DIMENSIONS  = "Dimensions";

        // Xml Inner Text Regex
        public const string INT_REGEX              = @"(\s*[+-]?(?<!\.)\b[0-9]+\b(?!\.[0-9])\s*)";
        public const string SPACE_REGEX            = @"((\s*)?,?(\s*)?)";
        public const string DIMENSIONS_SPECE_REGEX = @"((\s*)?(x|X)?(\s*)?)";

        #endregion

        public XmlElement LoadSheetTag { get; private set; }
        public Dictionary<string, List<Rectangle>> GroupTiles { get; private set; }  // <groupName, CropRect>>
        public Texture2D Texture { get; private set; }

        private string groupName = string.Empty;

        public LoadSheetReader(XmlElement tag)
        {
            LoadSheetTag = tag;
            GroupTiles = new Dictionary<string, List<Rectangle>>();
            Texture = null;
        }

        public void read()
        {
            foreach (var node in LoadSheetTag)
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
            foreach (XmlAttribute attrib in element.Attributes)
            {
                switch (attrib.Name)
                {
                    case NAME:
                        // probably should catch exception if no Name attribute found
                        if (attrib.Value.Contains(UriUtilities.URI_SEPARATOR))
                        {
                            Texture = AssetLoader.Load<Texture2D>(attrib.Value);
                        }
                        else
                        {
                            Texture = AssetLoader.LoadUsingExtension(attrib.Value) as Texture2D;
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private void ReadElementChildNodes(XmlElement parentElement)
        {
            foreach (var node in parentElement.ChildNodes)
            {
                var element = node as XmlElement;

                if (element == null)
                {
                    continue;
                }
                switch (element.Name)
                {
                    case TILE:
                        Parse_Tile(element, groupName);
                        break;

                    case TILE_GROUP:
                        groupName = string.Empty;
                        foreach (XmlAttribute attrib in element.Attributes)
                        {
                            if (attrib.Name == NAME)
                            {
                                groupName = attrib.Value;
                            }
                        }

                        ReadElementChildNodes(element);
                        break;

                    default:
                        break;
                }
            }
        }

        private void Parse_Tile(XmlElement element, string groupName)
        {
            Rectangle tile = new Rectangle(0, 0, Texture.Width, Texture.Height);

            if (!groupName.Equals(string.Empty))
            {
                groupName += ".";  // Add GroupName sparator if not in global tile group
            }

            if (!GroupTiles.ContainsKey(groupName))
            {
                GroupTiles.Add(groupName, new List<Rectangle>());
            }

            foreach (XmlAttribute attrib in element.Attributes)
            {
                switch (attrib.Name)
                {
                    case TOP_LEFT:
                        Match coords = Regex.Match(attrib.Value, INT_REGEX + SPACE_REGEX + INT_REGEX);
                        tile.X = Convert.ToInt32(coords.Groups[1].Value.Trim());
                        tile.Y = Convert.ToInt32(coords.Groups[5].Value.Trim());
                        break;

                    case DIMENSIONS:
                        Match dimensions = Regex.Match(attrib.Value, INT_REGEX + DIMENSIONS_SPECE_REGEX + INT_REGEX);
                        tile.Width = Convert.ToInt32(dimensions.Groups[1].Value.Trim());
                        tile.Height = Convert.ToInt32(dimensions.Groups[6].Value.Trim());
                        break;

                    default:
                        break;
                }
            }

            GroupTiles[groupName].Add(tile);
        }
    }
}
