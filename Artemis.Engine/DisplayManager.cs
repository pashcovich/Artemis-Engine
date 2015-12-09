#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Windows.Forms;

#endregion

namespace Artemis.Engine
{
    public class DisplayManager
    {

        // The amounts by which the display area is offset by the window border.
        public const int WINDOW_BORDER_OFFSET_X = 8;
        public const int WINDOW_BORDER_OFFSET_Y = 30;

        private GameKernel game;
        private GameProperties properties;
        private GraphicsDevice graphicsDevice;
        private GraphicsDeviceManager graphicsManager;
        private SpriteBatch spriteBatch;
        private GameWindow window;

        /// <summary>
        /// The current resolution of the window.
        /// </summary>
        public Resolution WindowResolution { get; private set; }

        /// <summary>
        /// Whether or not ReinitDisplayProperties has to be called. 
        /// </summary>
        private bool dirty;

        internal DisplayManager( GameKernel game
                               , GameProperties properties
                               , RenderPipeline renderPipeline)
        {
            this.game            = game;
            this.properties      = properties;
            this.graphicsDevice  = renderPipeline.GraphicsDevice;
            this.graphicsManager = renderPipeline.GraphicsDeviceManager;
            this.spriteBatch     = renderPipeline.SpriteBatch;

            window = game.Window;
            this.WindowResolution = properties.BaseResolution;

            ReinitDisplayProperties(true);
        }

        /// <summary>
        /// Initialize (or reinitialize) the properties of the screen form.
        /// </summary>
        internal void ReinitDisplayProperties(bool overrideDirty = false)
        {
            graphicsManager.SynchronizeWithVerticalRetrace = properties.VSync;

            if (!dirty && !overrideDirty)
            {
                return;
            }

            game.IsMouseVisible      = properties.MouseVisible;
            window.IsBorderless      = properties.Borderless;
            window.AllowUserResizing = properties.WindowResizable;

            graphicsManager.IsFullScreen              = properties.Fullscreen;
            graphicsManager.PreferredBackBufferWidth  = WindowResolution.Width;
            graphicsManager.PreferredBackBufferHeight = WindowResolution.Height;

            // Center the display based on the native resolution.
            var form = (Form)Control.FromHandle(window.Handle);
            var position = new System.Drawing.Point(
                (Resolution.Native.Width - WindowResolution.Width) / 2,
                (Resolution.Native.Height - WindowResolution.Height) / 2
                );

            // This offset seems to width and height of the windows border,
            // so it accounts for the slight off-centering (unless the window
            // is larger than the native display).
            if (!properties.Borderless)
            {
                position.X -= WINDOW_BORDER_OFFSET_X;
                position.Y -= WINDOW_BORDER_OFFSET_Y;
            }

            graphicsManager.ApplyChanges();

            /* We have to reposition the form after we apply changes to the graphics manager
             * otherwise if the user changes the resolution while in fullscreen, then goes into
             * windowed mode, the window would be position relative to the previous resolution
             * rather than the new resolution.
             */
            form.Location = position;

            dirty = false;
        }

        /// <summary>
        /// Toggle whether or not the display is fullscreen.
        /// </summary>
        public void ToggleFullscreen()
        {
            SetFullscreen(properties.Fullscreen);
        }

        /// <summary>
        /// Toggle whether or not the mouse is visible.
        /// </summary>
        public void ToggleMouseVisibility()
        {
            SetBorderless(!properties.MouseVisible);
        }

        /// <summary>
        /// Toggle whether or not the window form has a border.
        /// </summary>
        public void ToggleBorderless()
        {
            SetBorderless(!properties.Borderless);
        }

        /// <summary>
        /// Toggle whether or not vertical synchronization is used.
        /// </summary>
        public void ToggleVSync()
        {
            SetVSync(!properties.VSync);
        }

        /// <summary>
        /// Set whether or not the display is fullscreen.
        /// </summary>
        /// <param name="state"></param>
        public void SetFullscreen(bool state)
        {
            if (!properties.FullscreenTogglable)
                throw UntogglableException("Fullscreen", GameSetupReader.FULLSCREEN_TOGGLABLE_ELEMENT);
            properties.Fullscreen = state;
            dirty = true;
        }

        /// <summary>
        /// Set whether or not the mouse is visible.
        /// </summary>
        /// <param name="state"></param>
        public void SetMouseVisibility(bool state)
        {
            if (!properties.MouseVisibilityTogglable)
                throw UntogglableException("Mouse visibility", GameSetupReader.MOUSE_VISIBILITY_TOGGLABLE_ELEMENT);
            properties.MouseVisible = state;
            dirty = true;
        }

        /// <summary>
        /// Set whether or not the window form has a border.
        /// </summary>
        /// <param name="state"></param>
        public void SetBorderless(bool state)
        {
            if (!properties.MouseVisibilityTogglable)
                throw UntogglableException("Borderless", GameSetupReader.BORDER_TOGGLABLE_ELEMENT);
            properties.Borderless = state;
            dirty = true;
        }

        /// <summary>
        /// Set whether or not vertical synchronization is used.
        /// </summary>
        /// <param name="state"></param>
        public void SetVSync(bool state)
        {
            properties.VSync = state;
            dirty = true;
        }

        private static DisplayManagerException UntogglableException(string property, string element)
        {
            return new DisplayManagerException(
                String.Format(
                    "{0} is not set to be togglable. To change this, add a " +
                    "{1} element to the root element of the .gamesetup file.",
                    property, element
                    )
                );
        }

        /// <summary>
        /// Set the name of the window.
        /// </summary>
        /// <param name="name"></param>
        public void SetWindowTitle(string name)
        {
            properties.WindowTitle = name;
            window.Title = name;
        }
    }
}
