﻿#region Using Statements

using Microsoft.Xna.Framework;

#endregion

namespace Artemis.Engine
{

    /// <summary>
    /// A class representing global game properties.
    /// </summary>
    public sealed class GameProperties
    {

        #region Default Values

        // Defaults

        internal const bool DEFAULT_WINDOW_RESIZABLE           = false;
        internal const bool DEFAULT_FULLSCREEN                 = false;
        internal const bool DEFAULT_FULLSCREEN_TOGGLABLE       = false;
        internal const bool DEFAULT_MOUSE_VISIBLE              = true;
        internal const bool DEFAULT_MOUSE_VISIBILITY_TOGGLABLE = false;
        internal const bool DEFAULT_BORDERLESS                 = false;
        internal const bool DEFAULT_BORDER_TOGGLABLE           = false;
        internal const bool DEFAULT_VSYNC                      = false;
        internal const bool DEFAULT_FIXED_TIME_STEP            = true;
        internal const bool DEFAULT_STATIC_RESOLUTION          = false;
        internal const bool DEFAULT_STATIC_ASPECT_RATIO        = false;
        internal const bool DEFAULT_ONLY_LANDSCAPE_RESOLUTIONS = false;
        internal const bool DEFAULT_ONLY_PORTRAIT_RESOLUTIONS  = false;

        internal const int DEFAULT_FRAMERATE                   = 60;
        internal const string DEFAULT_CONTENT_FOLDER           = "Content";
        internal static readonly Color DEFAULT_BG_COLOUR       = Color.Black;
        internal static readonly Resolution DEFAULT_RESOLUTION = new Resolution(800, 600);

        #endregion

        // All the following have internal setters so that the user can't reset them
        // at will, but if necessary *within* the assembly a class can change them.

        /// <summary>
        /// The game's base resolution. The base resolution acts as a "default" resolution,
        /// from which every renderable item gets scaled relative to when the resolution changes.
        /// </summary>
        public Resolution BaseResolution { get; internal set; }

        /// <summary>
        /// Whether or not the game's window is resizable (via dragging by the user).
        /// 
        /// NOTE: If this is false this will not completely disallow changing the resolution,
        /// it will just disallow the user from clicking and dragging to change the resolution.
        /// If you want to completely disallow all resolution changes, then set StaticResolution
        /// to true.
        /// </summary>
        public bool WindowResizable { get; internal set; }

        /// <summary>
        /// Whether or not the game is fullscreen on startup.
        /// </summary>
        public bool Fullscreen { get; internal set; }

        /// <summary>
        /// Whether or not fullscreen is togglable.
        /// </summary>
        public bool FullscreenTogglable { get; internal set; }

        /// <summary>
        /// Whether or not the mouse is visible on startup.
        /// </summary>
        public bool MouseVisible { get; internal set; }

        /// <summary>
        /// Whether or not mouse visibility is togglable.
        /// </summary>
        public bool MouseVisibilityTogglable { get; internal set; }

        /// <summary>
        /// Whether or not the window is borderless on startup.
        /// </summary>
        public bool Borderless { get; internal set; }

        /// <summary>
        /// Whether or not the window border is togglable.
        /// </summary>
        public bool BorderTogglable { get; set; }

        /// <summary>
        /// The window title.
        /// </summary>
        public string WindowTitle { get; set; }

        /// <summary>
        /// Whether or not the game's renderer uses vertical synchronization.
        /// </summary>
        public bool VSync { get; internal set; }

        /// <summary>
        /// The background colour.
        /// </summary>
        public Color BackgroundColour { get; internal set; }

        /// <summary>
        /// The content folder name.
        /// </summary>
        public string ContentFolder { get; internal set; }

        /// <summary>
        /// Whether or not the game updates with a fixed time step. If this is
        /// false the game will update as fast as possible (i.e. no framerate limit).
        /// </summary>
        public bool FixedTimeStep { get; internal set; }

        /// <summary>
        /// The frame rate of the game (the FPS).
        /// </summary>
        public int FrameRate { get; internal set; }

        /// <summary>
        /// Whether or not the resolution can be changed.
        /// </summary>
        public bool StaticResolution { get; internal set; }

        /// <summary>
        /// If true, this will prevent the resolution from being changed to a different
        /// aspect ratio (width/height) than the base resolution.
        /// </summary>
        public bool StaticAspectRatio { get; internal set; }

        /// <summary>
        /// If true, this will prevent the resolution from being changed to a resolution
        /// that isn't landscape (width >= height).
        /// </summary>
        public bool OnlyLandscapeResolutions { get; internal set; }

        /// <summary>
        /// If true, this will prevent the resolution from being changed to a resolution
        /// that isn't portrait (width <= height).
        /// </summary>
        public bool OnlyPortraitResolutions { get; internal set; }

        // Internal because we don't want any little kiddies creating their own instances
        // of GameProperties and messing things up.
        internal GameProperties()
        {
            // Setup default values.
            BaseResolution           = DEFAULT_RESOLUTION;
            Fullscreen               = DEFAULT_FULLSCREEN;
            FullscreenTogglable      = DEFAULT_FULLSCREEN_TOGGLABLE;
            MouseVisible             = DEFAULT_MOUSE_VISIBLE;
            MouseVisibilityTogglable = DEFAULT_MOUSE_VISIBILITY_TOGGLABLE;
            Borderless               = DEFAULT_BORDERLESS;
            BorderTogglable          = DEFAULT_BORDER_TOGGLABLE;
            VSync                    = DEFAULT_VSYNC;
            BackgroundColour         = DEFAULT_BG_COLOUR;
            ContentFolder            = DEFAULT_CONTENT_FOLDER;
            FixedTimeStep            = DEFAULT_FIXED_TIME_STEP;
            FrameRate                = DEFAULT_FRAMERATE;
        }

    }
}
