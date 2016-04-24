#region Using Statements

using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

#endregion

namespace Artemis.Engine
{
    public class GameConstants
    {
        /// <summary>
        /// The default values of the game constants (if a constant isn't specified in the
        /// game constants XML file).
        /// </summary>
        public static class Defaults
        {
            public const bool FullscreenTogglable      = false;
            public const bool MouseVisibilityTogglable = false;
            public const bool BorderTogglable          = false;
            public const bool StaticResolution         = false;
            public const bool StaticAspectRatio        = false;
            public const bool OnlyLandscapeResolutions = false;
            public const bool OnlyPortraitResolutions  = false;
            public const bool FixedTimeStep            = true;

            public const bool DefaultFullscreen      = false;
            public const bool DefaultMouseVisibility = false;
            public const bool DefaultBorderless      = false;
            public const bool DefaultVSync           = false;

            public const int DefaultFrameRate = 60;
            public static readonly Resolution BaseResolution = new Resolution(800, 600);
            public static readonly Resolution DefaultResolution = new Resolution(800, 600);
            public const string ContentFolder = "Content";
            public const string WindowTitle = "Game";
            public static readonly Color BackgroundColour = Color.Black;

            public const bool DisableUserOptionsWriteOnClose = false;
        }

        public static class XmlElements
        {
            public const string ROOT = "GameConstants";

            public const string KERNEL_CONSTANTS         = "Kernel";
            public const string DISPLAY_CONSTANTS        = "Display";
            public const string OPTION_DEFAULT_CONSTANTS = "OptionDefaults";
            public const string DEBUG_CONSTANTS          = "Debug";

            public const string FULLSCREEN_TOGGLABLE       = "FullscreenTogglable";
            public const string MOUSE_VISIBILITY_TOGGLABLE = "MouseVisibilityTogglable";
            public const string BORDER_TOGGLABLE           = "BorderTogglable";
            public const string STATIC_RESOLUTION          = "StaticResolution";
            public const string STATIC_ASPECT_RATIO        = "StaticAspectRatio";
            public const string ONLY_LANDSCAPE_RESOLUTIONS = "OnlyLandscapeResolutions";
            public const string ONLY_PORTRAIT_RESOLUTIONS  = "OnlyPortraitResolutions";
            public const string FIXED_TIME_STEP            = "FixedTimeStep";
            public const string DEFAULT_FULLSCREEN         = "DefaultFullscreen";
            public const string DEFAULT_MOUSE_VISIBILITY   = "DefaultMouseVisibility";
            public const string DEFAULT_BORDERLESS         = "DefaultBorderless";
            public const string DEFAULT_VSYNC              = "DefaultVSync";
            public const string DEFAULT_FRAME_RATE         = "DefaultFrameRate";
            public const string BASE_RESOLUTION            = "BaseResolution";
            public const string DEFAULT_RESOLUTION         = "DefaultResolution";
            public const string CONTENT_FOLDER             = "ContentFolder";
            public const string WINDOW_TITLE               = "WindowTitle";
            public const string BACKGROUND_COLOUR          = "BackgroundColour";
            public const string DISABLE_USER_OPTIONS_WRITE_ON_CLOSE = "DisableUserOptionsWriteOnClose";
        }

        private static GameConstants Instance = new GameConstants();

        /// <summary>
        /// Whether or not DisplayManager.Fullscreen can be changed.
        /// </summary>
        public static bool FullscreenTogglable { get { return Instance.fullscreenTogglable; } }

        /// <summary>
        /// Whether or not DisplayManager.MouseVisible can be changed.
        /// </summary>
        public static bool MouseVisibilityTogglable { get { return Instance.mouseVisibilityTogglable; } }

        /// <summary>
        /// Whether or not DisplayManager.Borderless can be changed.
        /// </summary>
        public static bool BorderTogglable { get { return Instance.borderTogglable; } }

        /// <summary>
        /// Whether or not the resolution can be changed.
        /// </summary>
        public static bool StaticResolution { get { return Instance.staticResolution; } }

        /// <summary>
        /// Whether or not the resolution's aspect ratio can be changed (this does not prevent
        /// the resolution from changing entirely).
        /// </summary>
        public static bool StaticAspectRatio { get { return Instance.staticAspectRatio; } }

        /// <summary>
        /// Whether or not the resolution is restricted to only landscape resolutions.
        /// </summary>
        public static bool OnlyLandscapeResolutions { get { return Instance.onlyLandscapeResolutions; } }

        /// <summary>
        /// Whether or not the resolution is restricted to only portrait resolutions.
        /// </summary>
        public static bool OnlyPortraitResolutions { get { return Instance.onlyPortraitResolutions; } }

        /// <summary>
        /// Whether or not the game runs at a fixed time step.
        /// </summary>
        public static bool FixedTimeStep { get { return Instance.fixedTimeStep; } }

        /// <summary>
        /// The base resolution.
        /// </summary>
        public static Resolution BaseResolution { get { return Instance.baseResolution; } }

        /// <summary>
        /// The default value of DisplayManager.Fullscreen if an initial value for DisplayManager.Fullscreen
        /// isn't supplied in the user options file.
        /// </summary>
        public static bool DefaultFullscreen { get { return Instance.defaultFullscreen; } }

        /// <summary>
        /// The default value of DisplayManager.MouseVisible if an initial value for DisplayManager.MouseVisible
        /// isn't supplied in the user options file.
        /// </summary>
        public static bool DefaultMouseVisibility { get { return Instance.defaultMouseVisibility; } }

        /// <summary>
        /// The default value of DisplayManager.Borderless if an initial value for DisplayManager.Borderless
        /// isn't supplied in the user options file.
        /// </summary>
        public static bool DefaultBorderless { get { return Instance.defaultBorderless; } }

        /// <summary>
        /// The default value of DisplayManager.VSync if an initial value for DisplayManager.VSync
        /// isn't supplied in the user options file.
        /// </summary>
        public static bool DefaultVSync { get { return Instance.defaultVSync; } }

        /// <summary>
        /// The default value of GameKernel.FrameRate if an initial value isn't supplied int he user
        /// options file.
        /// </summary>
        public static int DefaultFrameRate { get { return Instance.defaultFrameRate; } }

        /// <summary>
        /// The default resolution if no initial resolution is supplied in the user options file.
        /// </summary>
        public static Resolution DefaultResolution { get { return Instance.defaultResolution; } }

        /// <summary>
        /// The location of the content folder.
        /// </summary>
        public static string ContentFolder { get { return Instance.contentFolder; } }

        /// <summary>
        /// The initial window title.
        /// </summary>
        public static string InitialWindowTitle { get { return Instance.windowTitle; } }

        /// <summary>
        /// The initial background colour.
        /// </summary>
        public static Color InitialBackgroundColour { get { return Instance.bgColour; } }

        /// <summary>
        /// Debug property: if enabled, prevents the UserOptions from being written to a file on
        /// exiting the game.
        /// </summary>
        public static bool DisableUserOptionsWriteOnClose { get { return Instance.disableUserOptionsWriteOnClose; } }

        private bool fullscreenTogglable      = Defaults.FullscreenTogglable; // display
        private bool mouseVisibilityTogglable = Defaults.MouseVisibilityTogglable;// display
        private bool borderTogglable          = Defaults.BorderTogglable;// display
        private bool staticResolution         = Defaults.StaticResolution;// display
        private bool staticAspectRatio        = Defaults.StaticAspectRatio;// display
        private bool onlyLandscapeResolutions = Defaults.OnlyLandscapeResolutions;// display
        private bool onlyPortraitResolutions  = Defaults.OnlyPortraitResolutions;// display
        private bool fixedTimeStep            = Defaults.FixedTimeStep;//kernel
        private bool defaultFullscreen        = Defaults.DefaultFullscreen;
        private bool defaultMouseVisibility   = Defaults.DefaultMouseVisibility;//options
        private bool defaultBorderless        = Defaults.DefaultBorderless;//options
        private bool defaultVSync             = Defaults.DefaultVSync;//options
        private int defaultFrameRate          = Defaults.DefaultFrameRate;//options
        private Resolution defaultResolution  = Defaults.DefaultResolution;//options
        private Resolution baseResolution     = Defaults.BaseResolution;// display
        private string contentFolder          = Defaults.ContentFolder;//kernel
        private string windowTitle            = Defaults.WindowTitle;//kernel
        private Color bgColour                = Defaults.BackgroundColour;// display
        private bool disableUserOptionsWriteOnClose = Defaults.DisableUserOptionsWriteOnClose;// debug

        private Dictionary<string, Action<string>> kernelConstantReaders
            = new Dictionary<string, Action<string>>();

        private Dictionary<string, Action<string>> displayConstantReaders
            = new Dictionary<string, Action<string>>();

        private Dictionary<string, Action<string>> optionDefaultsConstantReaders
            = new Dictionary<string, Action<string>>();

        private Dictionary<string, Action<string>> debugConstantReaders
            = new Dictionary<string, Action<string>>();

        private GameConstants() 
        {
            kernelConstantReaders.Add(XmlElements.CONTENT_FOLDER,  s => { contentFolder = s; });
            kernelConstantReaders.Add(XmlElements.FIXED_TIME_STEP, s => { fixedTimeStep = ReadBool(s, fixedTimeStep); });
            kernelConstantReaders.Add(XmlElements.WINDOW_TITLE,    s => { windowTitle   = s; });

            displayConstantReaders.Add(XmlElements.FULLSCREEN_TOGGLABLE,       s => { fullscreenTogglable      = ReadBool(s, fullscreenTogglable); });
            displayConstantReaders.Add(XmlElements.MOUSE_VISIBILITY_TOGGLABLE, s => { mouseVisibilityTogglable = ReadBool(s, mouseVisibilityTogglable); });
            displayConstantReaders.Add(XmlElements.BORDER_TOGGLABLE,           s => { borderTogglable          = ReadBool(s, borderTogglable); });
            displayConstantReaders.Add(XmlElements.STATIC_RESOLUTION,          s => { staticResolution         = ReadBool(s, staticResolution); });
            displayConstantReaders.Add(XmlElements.STATIC_ASPECT_RATIO,        s => { staticAspectRatio        = ReadBool(s, staticAspectRatio); });
            displayConstantReaders.Add(XmlElements.ONLY_LANDSCAPE_RESOLUTIONS, s => { onlyLandscapeResolutions = ReadBool(s, onlyLandscapeResolutions); });
            displayConstantReaders.Add(XmlElements.ONLY_PORTRAIT_RESOLUTIONS,  s => { onlyPortraitResolutions  = ReadBool(s, onlyPortraitResolutions); });
            displayConstantReaders.Add(XmlElements.BASE_RESOLUTION,            s => { baseResolution           = ReadResolution(s, baseResolution); });
            displayConstantReaders.Add(XmlElements.BACKGROUND_COLOUR,          s => { bgColour                 = ReadColour(s, bgColour); });

            optionDefaultsConstantReaders.Add(XmlElements.DEFAULT_FULLSCREEN,       s => { defaultFullscreen      = ReadBool(s, defaultFullscreen); });
            optionDefaultsConstantReaders.Add(XmlElements.DEFAULT_MOUSE_VISIBILITY, s => { defaultMouseVisibility = ReadBool(s, defaultMouseVisibility); });
            optionDefaultsConstantReaders.Add(XmlElements.DEFAULT_BORDERLESS,       s => { defaultBorderless      = ReadBool(s, defaultBorderless); });
            optionDefaultsConstantReaders.Add(XmlElements.DEFAULT_VSYNC,            s => { defaultVSync           = ReadBool(s, defaultVSync); });
            optionDefaultsConstantReaders.Add(XmlElements.DEFAULT_RESOLUTION,       s => { defaultResolution      = ReadResolution(s, defaultResolution); });
            optionDefaultsConstantReaders.Add(XmlElements.DEFAULT_FRAME_RATE,       s => { defaultFrameRate       = ReadInt(s, defaultFrameRate); });

            debugConstantReaders.Add(XmlElements.DISABLE_USER_OPTIONS_WRITE_ON_CLOSE, s => { disableUserOptionsWriteOnClose = ReadBool(s, disableUserOptionsWriteOnClose); });
        }

        private bool ReadBool(string s, bool defaultVal)
        {
            try { return Convert.ToBoolean(s); }
            catch (FormatException) { return defaultVal; }
        }

        private int ReadInt(string s, int defaultVal) 
        {
            try { return Convert.ToInt32(s); }
            catch (FormatException) { return defaultVal; }
        }

        private Resolution ReadResolution(string s, Resolution defaultVal)
        {
            try
            {
                var parts = s.Split('x');
                return new Resolution(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]));
            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is IndexOutOfRangeException)
                {
                    return defaultVal;
                }
                throw;
            }
        }

        private const string COLOUR_REGEX = @"0(x|X)[0-9a-fA-F]{6}$";

        private Color ReadColour(string s, Color defaultVal)
        {
            // If the given colour string is a name, attempt 
            // to find it as a static property of Color.

            // The binding flags when searching for the color
            // property in typeof(Color).
            var binding = BindingFlags.Public |
                          BindingFlags.Static |
                          BindingFlags.FlattenHierarchy;

            var prop = typeof(Color).GetProperty(s, binding);

            if (prop != null)
            {
                return (Color)prop.GetValue(null, null);
            }

            if (!Regex.IsMatch(s, COLOUR_REGEX))
            {
                // Log that we couldn't figure out the colour.
                return defaultVal;
            }

            var val = Convert.ToInt32(s, 16);

            var r = 0xff & (val >> 16);
            var g = 0xff & (val >> 8);
            var b = 0xff & val;

            return new Color(r, g, b);
        }

        public void Read(string fileName)
        {
            var doc = new XmlDocument();
            try
            {
                doc.Load(fileName);
            }
            catch (IOException)
            {
                return;
            }

            var root = doc.ChildNodes[1] as XmlElement;
            if (root.Name != XmlElements.ROOT)
            {
                // Should we throw an error or log something, or just fail silently?
                // return;
            }
            foreach (var child in root.ChildNodes)
            {
                var element = child as XmlElement;
                if (element == null)
                    continue;

                switch (element.Name)
                {
                    case XmlElements.KERNEL_CONSTANTS:
                        ReadElements(element, kernelConstantReaders);
                        break;
                    case XmlElements.DISPLAY_CONSTANTS:
                        ReadElements(element, displayConstantReaders);
                        break;
                    case XmlElements.OPTION_DEFAULT_CONSTANTS:
                        ReadElements(element, optionDefaultsConstantReaders);
                        break;
                    case XmlElements.DEBUG_CONSTANTS:
                        ReadElements(element, debugConstantReaders);
                        break;
                }
            }
        }

        public void ReadElements(XmlElement element, Dictionary<string, Action<string>> recognizedElementReaders)
        {
            foreach (var child in element.ChildNodes)
            {
                var e = child as XmlElement;
                if (e == null)
                    continue;
                if (recognizedElementReaders.ContainsKey(e.Name))
                {
                    recognizedElementReaders[e.Name](e.InnerText);
                }
            }
        }

        internal static void ReadFromFile(string fileName)
        {
            Instance.Read(fileName);
        }
    }
}
