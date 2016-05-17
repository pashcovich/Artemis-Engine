#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;

#endregion

namespace Artemis.Engine.Graphics.Animation
{
    public class SpriteSheetReader
    {
        #region Xml Constants

        // Xml Tags
        public const string LOAD_IMAGE_AS_TILE  = "LoadImageAsTile";
        public const string LOAD_SHEET          = "LoadSheet";
        public const string LOAD_DIRECTORY      = "LoadDirectory";
        public const string LOAD_DIRECTORY_FULL = "LoadDirectoryFull";

        // Xml Inner Tags
        public const string ADJUSTMENTS = "Adjustments";
        public const string CROP_TILE   = "CropTile";

        // Xml Attribute Names
        public const string NAME        = "Name";
        public const string TILE_GROUPS = "TileGroups";
        public const string TOP_LEFT    = "TopLeft";
        public const string DIMENSIONS  = "Dimensions";

        #endregion

        public XmlElement SpriteSheet { get; private set; }
        public SpriteSheet Sheet { get; private set; }
        public Dictionary<string, Dictionary<string, SpriteSheet.Tile>> TileGroups { get; private set; }  // <GroupName, Tile<Name, Tile (struct)>>
        public Dictionary<string, int> CurrentFrame { get; private set; }
        public List<Texture2D> Textures { get; private set; }

        private int currentTileID = 0;
        private SpriteSheet.Tile addTile;

        public SpriteSheetReader(XmlElement sheet)
        {
            SpriteSheet = sheet;
            TileGroups = new Dictionary<string, Dictionary<string, SpriteSheet.Tile>>();
            TileGroups.Add("", new Dictionary<string, SpriteSheet.Tile>());
            CurrentFrame = new Dictionary<string, int>();
            Textures = new List<Texture2D>();
        }

        public void Load()
        {
            foreach (var node in SpriteSheet)
            {
                var element = node as XmlElement;

                if (element == null)
                {
                    continue;
                }

                ReadElement(element);
            }

            Sheet = new SpriteSheet(Textures, TileGroups);
        }

        private void ReadElement(XmlElement element)
        {
            switch (element.Name)
            {
                case LOAD_IMAGE_AS_TILE:
                    LoadImageAsTileReader imageTileReader = new LoadImageAsTileReader(element);
                    imageTileReader.read();

                    addTile = new SpriteSheet.Tile(currentTileID, imageTileReader.Tile);
                    
                    if (!TileGroups.ContainsKey(imageTileReader.TileGroup))
                    {
                        TileGroups.Add(imageTileReader.TileGroup, new Dictionary<string, SpriteSheet.Tile>());
                    }

                    if (!CurrentFrame.ContainsKey(imageTileReader.TileGroup))
                    {
                        CurrentFrame.Add(imageTileReader.TileGroup, 0);
                    }

                    TileGroups[imageTileReader.TileGroup].Add(imageTileReader.TileGroup + CurrentFrame[imageTileReader.TileGroup], addTile);
                    Textures.Add(imageTileReader.Texture);

                    CurrentFrame[imageTileReader.TileGroup]++;
                    currentTileID++;
                    break;

                case LOAD_SHEET:
                    LoadSheetReader sheetReader = new LoadSheetReader(element);
                    sheetReader.read();

                    foreach (KeyValuePair<string, List<Rectangle>> group in sheetReader.GroupTiles)
                    {
                        if (!TileGroups.ContainsKey(group.Key))
                        {
                            TileGroups.Add(group.Key, new Dictionary<string, SpriteSheet.Tile>());
                            CurrentFrame.Add(group.Key, 0);
                        }
                        foreach (Rectangle tile in group.Value)
                        {
                            addTile = new SpriteSheet.Tile(currentTileID, tile);
                            TileGroups[group.Key].Add(group.Key + CurrentFrame[group.Key], addTile);
                            CurrentFrame[group.Key]++;
                        }
                    }

                    Textures.Add(sheetReader.Texture);
                    currentTileID++;
                    break;

                case LOAD_DIRECTORY:
                    LoadDirectoryReader directoryReader = new LoadDirectoryReader(element);
                    directoryReader.read();

                    if (!TileGroups.ContainsKey(directoryReader.GroupName))
                    {
                        TileGroups.Add(directoryReader.GroupName, new Dictionary<string, SpriteSheet.Tile>());
                        CurrentFrame.Add(directoryReader.GroupName, 0);
                    }

                    foreach (var key in directoryReader.Tiles.Keys)
                    {
                        addTile = new SpriteSheet.Tile(currentTileID, directoryReader.Tiles[key]);

                        TileGroups[directoryReader.GroupName].Add(directoryReader.GroupName + CurrentFrame[directoryReader.GroupName], addTile);
                        Textures.Add(directoryReader.Textures[key]);

                        CurrentFrame[directoryReader.GroupName]++;
                        currentTileID++;
                    }
                    break;

                case LOAD_DIRECTORY_FULL:
                    LoadDirectoryFullReader directoryFullReader = new LoadDirectoryFullReader(element);
                    directoryFullReader.Read();
                    
                    if (!TileGroups.ContainsKey(directoryFullReader.GroupName))
                    {
                        TileGroups.Add(directoryFullReader.GroupName, new Dictionary<string, SpriteSheet.Tile>());
                        CurrentFrame.Add(directoryFullReader.GroupName, 0);
                    }

                    foreach (var key in directoryFullReader.Tiles.Keys)
                    {
                        addTile = new SpriteSheet.Tile(currentTileID, directoryFullReader.Tiles[key]);

                        TileGroups[directoryFullReader.GroupName].Add(directoryFullReader.GroupName + CurrentFrame[directoryFullReader.GroupName], addTile);
                        Textures.Add(directoryFullReader.Textures[key]);

                        CurrentFrame[directoryFullReader.GroupName]++;
                        currentTileID++;
                    }
                    break;

                default:
                    break;
            }
        }
    }
}