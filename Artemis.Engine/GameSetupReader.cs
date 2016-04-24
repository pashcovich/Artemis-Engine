#region Using Statements

using Microsoft.Xna.Framework;

using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

#endregion

namespace Artemis.Engine
{

    /// <summary>
    /// A reader for the GameProperties.
    /// </summary>
    internal class GameSetupReader
    {

        #region Xml Constants

        // Xml Tags
        public const string BASE_RESOLUTION_ELEMENT            = "BaseResolution";
        public const string WINDOW_RESIZABLE_ELEMENT           = "WindowResizable";
        // public const string FULLSCREEN_ELEMENT                 = "Fullscreen";
        public const string FULLSCREEN_TOGGLABLE_ELEMENT       = "FullscreenTogglable";
        // public const string MOUSE_VISIBLE_ELEMENT              = "MouseVisible";
        public const string MOUSE_VISIBILITY_TOGGLABLE_ELEMENT = "MouseVisibilityTogglable";
        // public const string BORDERLESS_ELEMENT                 = "Borderless";
        public const string BORDER_TOGGLABLE_ELEMENT           = "BorderTogglable";
        public const string WINDOW_TITLE_ELEMENT               = "WindowTitle";
        // public const string VSYNC_ELEMENT                      = "VSync";
        public const string BG_COLOUR_ELEMENT                  = "BackgroundColour";
        public const string CONTENT_FOLDER_ELEMENT             = "ContentFolder";
        // public const string FIXED_TIME_STEP_ELEMENT            = "FixedTimeStep";
        // public const string FRAMERATE_ELEMENT                  = "FrameRate";
        public const string STATIC_ASPECT_RATIO_ELEMENT        = "StaticAspectRatio";
        public const string STATIC_RESOLUTION_ELEMENT          = "StaticResolution";
        public const string ONLY_LANDSCAPE_RESOLUTIONS_ELEMENT = "OnlyLandscapeResolutions";
        public const string ONLY_PORTRAIT_RESOLUTIONS_ELEMENT  = "OnlyPortraitResolutions";

        // Xml Inner Text Regexs
        public const string RESOLUTION_REGEX = @"[0-9]+x[0-9]+$";
        public const string COLOUR_REGEX     = @"0(x|X)[0-9a-fA-F]{6}$";

        // Xml Inner Text Identifiers (magic strings that denote special values when used
        // as the inner text in an Xml tag).
        public const string NATIVE_RESOLUTION_ID = "Native";

        #endregion

        /// <summary>
        /// The name of the setup file, relative to the application startup directory.
        /// </summary>
        public string SetupFileName { get; private set; }

        public GameSetupReader(string fileName)
        {
            SetupFileName = fileName;
        }

        /// <summary>
        /// Read the setup file and return a GameProperties object.
        /// </summary>
        /// <returns></returns>
        public GameProperties Read()
        {
            var properties = new GameProperties();

            var setupFile = new XmlDocument();
            try
            {
                setupFile.Load(SetupFileName);
            }
            catch (IOException)
            {
                throw new IOException(
                    String.Format(
                        "The setup file with name '{0}' could not be loaded. The most common causes for this error are " +
                        "either the path was misspelled, or the 'Copy to Output Directory' property of your setup file " +
                        "is not set to 'Copy if newer'. To change this in VS, right click on your setup file in the solution " +
                        "explorer, select properties, and change the 'Copy to Output Directory' property to 'Copy if newer'.",
                        SetupFileName
                        )
                    );
            }

            XmlElement root;
            try
            {
                root = setupFile.ChildNodes[1] as XmlElement;
            }
            catch (IndexOutOfRangeException)
            {
                // LOG: Could not load setup file, invalid Xml structure.
                return properties;
            }
            
            foreach (var node in root.ChildNodes)
            {
                var element = node as XmlElement;

                // Just continue if we don't know what we're parsing.
                if (element == null)
                {
                    continue;
                }

                ReadElementAsGameProperty(element, properties);       
            }

            return properties;
        }

        /// <summary>
        /// Read an XmlElement found in the document root and apply it
        /// to the GameProperties object.
        /// </summary>
        private void ReadElementAsGameProperty(XmlElement element, GameProperties properties)
        {
            switch (element.Name)
            {
                case BASE_RESOLUTION_ELEMENT:
                    properties.BaseResolution = ReadResolution(
                        element, GameProperties.DEFAULT_RESOLUTION);
                    break;
                case WINDOW_RESIZABLE_ELEMENT:
                    properties.WindowResizable = ReadBool(
                        element, GameProperties.DEFAULT_WINDOW_RESIZABLE);
                    break;
                case FULLSCREEN_TOGGLABLE_ELEMENT:
                    properties.FullscreenTogglable = ReadBool(
                        element, GameProperties.DEFAULT_FULLSCREEN_TOGGLABLE);
                    break;
                case MOUSE_VISIBILITY_TOGGLABLE_ELEMENT:
                    properties.MouseVisibilityTogglable = ReadBool(
                        element, GameProperties.DEFAULT_MOUSE_VISIBILITY_TOGGLABLE);
                    break;
                case BORDER_TOGGLABLE_ELEMENT:
                    properties.BorderTogglable = ReadBool(
                        element, GameProperties.DEFAULT_BORDER_TOGGLABLE);
                    break;
                case BG_COLOUR_ELEMENT:
                    properties.BackgroundColour = ReadColour(
                        element, GameProperties.DEFAULT_BG_COLOUR);
                    break;
                case CONTENT_FOLDER_ELEMENT:
                    properties.ContentFolder = element.InnerText;
                    break;
                case WINDOW_TITLE_ELEMENT:
                    properties.WindowTitle = element.InnerText;
                    break;
                case STATIC_RESOLUTION_ELEMENT:
                    properties.StaticResolution = ReadBool(
                        element, GameProperties.DEFAULT_STATIC_RESOLUTION);
                    break;
                case STATIC_ASPECT_RATIO_ELEMENT:
                    properties.StaticAspectRatio = ReadBool(
                        element, GameProperties.DEFAULT_STATIC_ASPECT_RATIO);
                    break;
                case ONLY_LANDSCAPE_RESOLUTIONS_ELEMENT:
                    properties.OnlyLandscapeResolutions = ReadBool(
                        element, GameProperties.DEFAULT_ONLY_LANDSCAPE_RESOLUTIONS);
                    break;
                case ONLY_PORTRAIT_RESOLUTIONS_ELEMENT:
                    properties.OnlyPortraitResolutions = ReadBool(
                        element, GameProperties.DEFAULT_ONLY_PORTRAIT_RESOLUTIONS);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Read a boolean value from an XmlElement.
        /// </summary>
        private bool ReadBool(XmlElement element, bool defaultValue)
        {
            var text = element.InnerText;
            bool val;
            try
            {
                val = Convert.ToBoolean(text);
            }
            catch (FormatException)
            {
                // Log that we couldn't get the value.
                return defaultValue;
            }
            return val;
        }

        private int ReadInt(XmlElement element, int defaultValue)
        {
            var text = element.InnerText;
            int val;
            try
            {
                val = Convert.ToInt32(text);
            }
            catch (FormatException)
            {
                return defaultValue;
            }
            return val;
        }

        /// <summary>
        /// Read a Resolution object from an XmlElement.
        /// </summary>
        private Resolution ReadResolution(XmlElement element, Resolution defaultValue)
        {
            var text = element.InnerText;

            if (text == NATIVE_RESOLUTION_ID)
            {
                return Resolution.Native;
            }

            if (!Regex.IsMatch(text, RESOLUTION_REGEX))
            {
                // Log that we couldn't figure out the resolution.
                return defaultValue;
            }

            var parts = text.Split('x');

            var width  = Int32.Parse(parts[0]);
            var height = Int32.Parse(parts[1]);

            return new Resolution(width, height);
        }

        /// <summary>
        /// Read a Color object from an XmlElement.
        /// </summary>
        private Color ReadColour(XmlElement element, Color defaultValue)
        {
            var text = element.InnerText;
                
            // If the given colour string is a name, attempt 
            // to find it as a static property of Color.

            // The binding flags when searching for the color
            // property in typeof(Color).
            var binding = BindingFlags.Public | 
                          BindingFlags.Static | 
                          BindingFlags.FlattenHierarchy;

            var prop = typeof(Color).GetProperty(text, binding);

            if (prop != null)
            {
                return (Color)prop.GetValue(null, null);
            }

            if (!Regex.IsMatch(text, COLOUR_REGEX))
            {
                // Log that we couldn't figure out the colour.
                return defaultValue;
            }

            var val = Convert.ToInt32(text, 16);

            var r = 0xff & (val >> 16);
            var g = 0xff & (val >> 8);
            var b = 0xff & val;

            return new Color(r, g, b);
        }

        /// <summary>
        /// Ensure that none of the properties are self-conflicting.
        /// </summary>
        /// <param name="properties"></param>
        public void CheckForContradictions(GameProperties properties)
        {
            if (properties.WindowResizable && properties.StaticResolution)
            {
                throw new GameSetupException(
                    string.Format(
                        "The game properties '{0}' and '{1}' cannot both be true.",
                        WINDOW_RESIZABLE_ELEMENT, STATIC_RESOLUTION_ELEMENT
                        )
                    );
            }

            if (properties.OnlyLandscapeResolutions && properties.OnlyPortraitResolutions)
            {
                throw new GameSetupException(
                    string.Format(
                        "The game properties '{0}' and '{1}' cannot both be true.",
                        ONLY_LANDSCAPE_RESOLUTIONS_ELEMENT, ONLY_PORTRAIT_RESOLUTIONS_ELEMENT
                        )
                    );
            }

            if (properties.OnlyLandscapeResolutions && !properties.BaseResolution.IsLandscape)
            {
                throw new GameSetupException(
                    string.Format(
                        "Since '{0}' is true, the base resolution ({1}) cannot be portrait (width < height).",
                        ONLY_LANDSCAPE_RESOLUTIONS_ELEMENT, properties.BaseResolution
                        )
                    );
            }

            if (properties.OnlyPortraitResolutions && !properties.BaseResolution.IsPortrait)
            {
                throw new GameSetupException(
                    string.Format(
                        "Since '{0}' is true, the base resolution ({1}) cannot be landscape (width > height).",
                        ONLY_PORTRAIT_RESOLUTIONS_ELEMENT, properties.BaseResolution
                        )
                    );
            }
        }

    }
}
