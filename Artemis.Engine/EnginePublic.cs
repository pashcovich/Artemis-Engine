#region Using Statements

using Artemis.Engine.Input;
using Artemis.Engine.Multiforms;
using Artemis.Engine.Persistence;
using Artemis.Engine.Utilities.Dynamics;
using Artemis.Engine.Utilities.Serialize;

using Microsoft.Xna.Framework;

using System;

#endregion

namespace Artemis.Engine
{

    /// <summary>
    /// The part of the Engine object which is publically available to users.
    /// </summary>
    public sealed partial class ArtemisEngine
    {
        /// <summary>
        /// The singleton instance of Engine.
        /// </summary>
        private static ArtemisEngine Instance;

        public static bool SetupCalled { get; private set; }

        /// <summary>
        /// Whether or not Engine.Setup has been called.
        /// </summary>
        public static bool BeginCalled { get { return Instance != null; } }

        /// <summary>
        /// The engine's global render pipeline. Controls all rendering that takes
        /// place in the game.
        /// </summary>
        public static RenderPipeline RenderPipeline { get { return Instance._RenderPipeline; } }

        /// <summary>
        /// The engine's global multiform manager.
        /// </summary>
        public static MultiformManager MultiformManager { get { return Instance._MultiformManager; } }

        /// <summary>
        /// The display manager which handles all properties of the display.
        /// </summary>
        public static DisplayManager DisplayManager { get { return Instance._DisplayManager; } }

        /// <summary>
        /// The global game timer which records total elapsed game time, frames passed, 
        /// and elapsed time since the last update.
        /// </summary>
        public static GlobalTimer GameTimer { get { return Instance._GameTimer; } }

        /// <summary>
        /// The global game updater which is in charge of remembering and
        /// updating ArtemisObjects.
        /// </summary>
        public static GlobalUpdater GameUpdater { get { return Instance._GameUpdater; } }

        /// <summary>
        /// The global game mouse input provider.
        /// </summary>
        public static MouseInput Mouse { get { return Instance._Mouse; } }

        /// <summary>
        /// The global game keyboard input provider.
        /// </summary>
        public static KeyboardInput Keyboard { get { return Instance._Keyboard; } }

        public static void Setup(string setupFileName, Action setupAction)
        {
            GameConstants.ReadFromFile(setupFileName);
            AddOptions();
            setupAction();

            SetupCalled = true;
        }

        public static void Setup(string setupFileName, string optionFileName)
        {
            GameConstants.ReadFromFile(setupFileName);
            UserOptions.SetFileName(optionFileName);
            AddOptions();

            SetupCalled = true;
        }

        public static void Begin(Action initializer)
        {
            if (!SetupCalled)
            {
                throw new EngineSetupException("Engine.Setup has not yet been called.");
            }
            if (BeginCalled)
            {
                throw new EngineSetupException("Engine.Begin called multiple times.");
            }
            Instance = new ArtemisEngine(initializer);
            Instance.Run();

            // Anything that happens after the above line will happen after the game window closes.

            if (!GameConstants.DisableUserOptionsWriteOnClose)
                UserOptions.Write();
        }

        /// <summary>
        /// Register multiforms to the engine.
        /// </summary>
        /// <param name="multiforms"></param>
        public static void RegisterMultiforms(params object[] multiforms)
        {
            if (!BeginCalled)
            {
                throw new EngineSetupException(
                    "Must call Engine.Setup before call to Engine.RegisterMultiforms.");
            }
            Instance._RegisterMultiforms(multiforms);
        }

        /// <summary>
        /// Indicate what multiform to construct upon game startup.
        /// </summary>
        /// <param name="multiform"></param>
        public static void StartWith(string multiformName)
        {
            if (!BeginCalled)
            {
                throw new EngineSetupException(
                    "Must call Engine.Setup before call to Engine.StartWith.");
            }
            MultiformManager.Activate(null, multiformName, new MultiformConstructionArgs(null));
        }

        #region Universal Game Options

        private static void AddOptions()
        {
            UserOptions.AddOption(
                new SimpleOptionRecord(
                    UserOptions.Builtins.Fullscreen,
                    GameConstants.DefaultFullscreen,
                    new BoolStringSerializer()));

            UserOptions.AddOption(
                new SimpleOptionRecord(
                    UserOptions.Builtins.MouseVisible,
                    GameConstants.DefaultMouseVisibility,
                    new BoolStringSerializer()));

            UserOptions.AddOption(
                new SimpleOptionRecord(
                    UserOptions.Builtins.Borderless,
                    GameConstants.DefaultBorderless,
                    new BoolStringSerializer()));

            UserOptions.AddOption(
                new SimpleOptionRecord(
                    UserOptions.Builtins.Resolution,
                    GameConstants.DefaultResolution,
                    new ResolutionStringSerializer()));

            UserOptions.AddOption(
                new SimpleOptionRecord(
                    UserOptions.Builtins.VSync,
                    GameConstants.DefaultVSync,
                    new BoolStringSerializer()));

            UserOptions.AddOption(
                new SimpleOptionRecord(
                    UserOptions.Builtins.FrameRate,
                    GameConstants.DefaultFrameRate,
                    new Int32StringSerializer()));

            UserOptions.AddOptionChangedEventHandler(UserOptions.Builtins.FrameRate, 
                (s, e) => { Instance.gameKernel.FrameRate = UserOptions.Get<int>(UserOptions.Builtins.FrameRate); });
        }

        #endregion
    }
}
