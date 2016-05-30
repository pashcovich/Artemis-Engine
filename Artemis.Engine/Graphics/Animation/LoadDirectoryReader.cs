#region Using Statements

using Artemis.Engine.Assets;
using Artemis.Engine.Utilities.UriTree;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

#endregion

namespace Artemis.Engine.Graphics.Animation
{
    public class LoadDirectoryReader
    {
        #region Xml Constants

        // Xml Tags
        public const string ADJUSTMENTS = "Adjustments";
        public const string CROP_TILE   = "CropTile";

        // Xml Attribute Names
        public const string NAME       = "Name";
        public const string TILE_GROUP = "TileGroup";
        public const string TOP_LEFT   = "TopLeft";
        public const string DIMENSIONS = "Dimensions";

        // Xml Inner Text Regex
        public const string INT_REGEX              = @"(\s*[+-]?(?<!\.)\b[0-9]+\b(?!\.[0-9])\s*)";
        public const string SPACE_REGEX            = @"((\s*)?,?(\s*)?)";
        public const string DIMENSIONS_SPECE_REGEX = @"((\s*)?(x|X)?(\s*)?)";

        // Image Extensions
        public readonly List<string> IMAGE_EXTENSIONS = new List<string> { ".JPG", ".PNG" };

        #endregion

        public XmlElement LoadImageTag { get; private set; }
        public Dictionary<string, Rectangle> Tiles { get; private set; }
        public Dictionary<string, Texture2D> Textures { get; private set; }
        public string GroupName { get; private set; }

        private string selectedImage = "";

        public LoadDirectoryReader(XmlElement tag)
        {
            LoadImageTag = tag;
            Tiles = new Dictionary<string, Rectangle>();
            GroupName = string.Empty;
            Textures = new Dictionary<string, Texture2D>();
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
            Rectangle tile = new Rectangle(0, 0, 0, 0);
            foreach (XmlAttribute attrib in element.Attributes)
            {
                switch (attrib.Name)
                {
                    case NAME:
                        if (Tiles.ContainsKey(Path.GetFileNameWithoutExtension(attrib.Value)))
                        {
                            selectedImage = Path.GetFileNameWithoutExtension(attrib.Value);
                            tile.Width = Textures[selectedImage].Width;
                            tile.Height = Textures[selectedImage].Height;
                        }
                        else
                        {
                            LoadFromDirectory(attrib.Value);
                        }
                        break;

                    case TILE_GROUP:
                        GroupName = element.Value;
                        break;

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
            Tiles.Add(selectedImage, tile);
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

        private void LoadFromDirectory(string dir)
        {
            foreach (string path in Directory.GetFiles(dir))
            {
                if (IMAGE_EXTENSIONS.Contains(Path.GetExtension(path).ToUpper()) && Directory.Exists(path))
                {
                    if (path.Contains(UriUtilities.URI_SEPARATOR))
                    {
                        Textures.Add(Path.GetFileNameWithoutExtension(path), AssetLoader.Load<Texture2D>(path));
                    }
                    else
                    {
                        Textures.Add(Path.GetFileNameWithoutExtension(path), AssetLoader.LoadUsingExtension(path) as Texture2D);
                    }
                }
            }
        }
    }
}
