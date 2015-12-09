#region Using Statements

using Artemis.Engine.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

#endregion

namespace Artemis.Engine
{
    public sealed partial class ArtemisEngine
    {
        // Private instance fields

        internal RenderPipeline   _RenderPipeline   { get; private set; }
        internal MultiformManager _MultiformManager { get; private set; }
        internal GameProperties   _GameProperties   { get; private set; }
        internal DisplayManager   _DisplayManager   { get; private set; }
        internal GlobalTimer      _GameTimer        { get; private set; }
        internal GlobalUpdater    _GameUpdater      { get; private set; }
        internal MouseInput       _Mouse            { get; private set; }
        internal KeyboardInput    _Keyboard         { get; private set; }

        /// <summary>
        /// The instance of the actual Monogame Game object.
        /// </summary>
        private GameKernel gameKernel;

        /// <summary>
        /// The action that is performed when the game first starts up. Due to the
        /// way Monogame works, everything related to initializing the game has to
        /// be done on the first frame of updating the game, so it's easier if the
        /// initialization routine is supplied as an Action to be performed.
        /// </summary>
        private Action initializer;

        /// <summary>
        /// Whether or not the initializer has been called.
        /// </summary>
        internal bool Initialized { get; private set; }

        private ArtemisEngine(GameProperties properties, Action initializer) : base()
        {
            this.initializer = initializer;
            Initialized = false;

            _GameProperties = properties;
            gameKernel = new GameKernel(this);

            _MultiformManager = new MultiformManager();
            _GameTimer        = new GlobalTimer();
            _GameUpdater      = new GlobalUpdater();
            _Mouse            = new MouseInput();
            _Keyboard         = new KeyboardInput();
        }

        /// <summary>
        /// Initialize the game's render pipeline. This has to be called in gameKernel.Initialize
        /// because that's where the spriteBatch has to be created.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="gd"></param>
        /// <param name="gdm"></param>
        internal void InitializeRenderPipeline(SpriteBatch sb, GraphicsDevice gd, GraphicsDeviceManager gdm)
        {
            _RenderPipeline = new RenderPipeline(sb, gd, gdm);
            _DisplayManager = new DisplayManager(gameKernel, _GameProperties, _RenderPipeline);
        }

        /// <summary>
        /// Actually call the initializer.
        /// </summary>
        internal void Initialize()
        {
            initializer();
            Initialized = true;
        }

        private void Run()
        {
            using (gameKernel)
            {
                gameKernel.Run();
            }
        }

        /// <summary>
        /// The main game loop.
        /// </summary>
        /// <param name="gameTime"></param>
        internal void Update(GameTime gameTime)
        {
            // Update the time first...
            _GameTimer.UpdateTime(gameTime);

            // Then the input...
            _Mouse.Update();
            _Keyboard.Update();

            // Then the multiforms...
            _MultiformManager.Update();

            // And finally all remaining ArtemisObjects.
            _GameUpdater.FinalizeUpdate();
        }

        /// <summary>
        /// The main rendering loop, which gets called after Update.
        /// </summary>
        /// <param name="gameTime"></param>
        internal void Render()
        {
            // Reinitialize the display first...
            _DisplayManager.ReinitDisplayProperties();

            // Then begin the render cycle...
            _RenderPipeline.BeginRenderCycle();

            // Then render everything...
            _MultiformManager.Render();

            // Then end the render cycle.
            _RenderPipeline.EndRenderCycle();
        }

        /// <summary>
        /// Register Multiform classes to the engine's MultiformManager.
        /// </summary>
        /// <param name="multiforms"></param>
        private void _RegisterMultiforms(object[] multiforms)
        {
            var i = 0;
            foreach (var multiform in multiforms)
            {
                if (multiform is Multiform)
                {
                    MultiformManager.RegisterMultiform((Multiform)multiform);
                }
                else if (multiform is Type)
                {
                    MultiformManager.RegisterMultiform((Type)multiform);
                }
                else
                {
                    throw new MultiformRegistrationException(
                        String.Format(
                            "When registering multiforms, the given objects must either be a multiform " +
                            "instance or a multiform type. The object at index {0} was neither (received {1}).",
                            i, multiform));
                }
                i++;
            }
        }
    }
}
