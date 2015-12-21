#region Using Statements

using Artemis.Engine.Utilities;

using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Artemis.Engine.Input
{

    /// <summary>
    /// The main keyboard input provider class.
    /// </summary>
    public sealed class KeyboardInput
    {

        private static List<Keys> AllKeys;

        static KeyboardInput()
        {
            AllKeys = Reflection.EnumItems<Keys>();
        }

        /// <summary>
        /// The number of frames since a key was pressed.
        /// </summary>
        public Dictionary<Keys, int> FramesSinceKeyPressed { get; private set; }

        /// <summary>
        /// The number of frames since a key was released.
        /// </summary>
        public Dictionary<Keys, int> FramesSinceKeyReleased { get; private set; }

        /// <summary>
        /// The number of frames since the last keyboard activity (i.e. any key pressed).
        /// </summary>
        public int FramesSinceLastKeyboardActivity { get; private set; }

        public KeyboardInput() 
        {
            FramesSinceKeyPressed  = new Dictionary<Keys, int>();
            FramesSinceKeyReleased = new Dictionary<Keys, int>();

            FramesSinceLastKeyboardActivity = 0;

            foreach (var key in AllKeys)
            {
                FramesSinceKeyPressed.Add(key, 0);
                FramesSinceKeyReleased.Add(key, 0);
            }
        }

        internal void Update() 
        {
            var keyboardState = Keyboard.GetState();

            bool anyKeyPressed = false;
            foreach (var key in AllKeys)
            {
                if (keyboardState.IsKeyDown(key))
                {
                    FramesSinceKeyPressed[key]++;
                    FramesSinceKeyReleased[key] = 0;
                    anyKeyPressed = true;
                }
                else
                {
                    FramesSinceKeyPressed[key] = 0;
                    FramesSinceKeyReleased[key]++;
                }
            }

            if (anyKeyPressed)
            {
                FramesSinceLastKeyboardActivity = 0;
            }
            else
            {
                FramesSinceLastKeyboardActivity++;
            }
        }

        /// <summary>
        /// Check if a key has been clicked (i.e. has been pressed for exactly 1 frame).
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsClicked(Keys key)
        {
            return FramesSinceKeyPressed[key] == 1;
        }

        /// <summary>
        /// Check if a combination of keys have been clicked (i.e. has been 
        /// pressed for exactly 1 frame).
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsClicked(KeyCombination keys)
        {
            return keys.Keys.All(IsClicked);
        }

        /// <summary>
        /// Check if a key has been pressed for any number of keys.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsHeld(Keys key)
        {
            return FramesSinceKeyPressed[key] > 0;
        }

        /// <summary>
        /// Check if a combination of keys have been pressed for any number of keys.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsHeld(KeyCombination keys)
        {
            return keys.Keys.All(IsHeld);
        }

        /// <summary>
        /// Check if a key has been pressed for exactly the given number of frames.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="frames"></param>
        /// <returns></returns>
        public bool IsHeldFor(Keys key, int frames)
        {
            return FramesSinceKeyPressed[key] == frames;
        }

        /// <summary>
        /// Check if a combination of keys have been pressed for exactly the given number of frames.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="frames"></param>
        /// <returns></returns>
        public bool IsHeldFor(KeyCombination keys, int frames)
        {
            return keys.Keys.All(k => IsHeldFor(k, frames));
        }

        /// <summary>
        /// Check if a key has been pressed for at least the given number of frames.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="frames"></param>
        /// <returns></returns>
        public bool IsHeldForAtleast(Keys key, int frames)
        {
            return FramesSinceKeyPressed[key] >= frames;
        }

        /// <summary>
        /// Check if a combination of keys have been pressed for at least the given number of frames.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="frames"></param>
        /// <returns></returns>
        public bool IsHeldForAtleast(KeyCombination keys, int frames)
        {
            return keys.Keys.All(k => IsHeldForAtleast(k, frames));
        }

        /// <summary>
        /// Check if a key has been released (i.e. has been unpressed for exactly 1 frame).
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsReleased(Keys key)
        {
            return FramesSinceKeyReleased[key] == 1;
        }

        /// <summary>
        /// Check if a combination of keys have been released (i.e. has been unpressed for exactly 1 frame).
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsReleased(KeyCombination keys)
        {
            return keys.Keys.All(IsReleased);
        }

        /// <summary>
        /// Check if a key has been unpressed for any number of frames.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsUnheld(Keys key)
        {
            return FramesSinceKeyReleased[key] > 0;
        }

        /// <summary>
        /// Check if a combination of keys have been unpressed for any number of frames.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsUnheld(KeyCombination keys)
        {
            return keys.Keys.All(IsUnheld);
        }

        /// <summary>
        /// Check if a key has been unpressed for exactly the given number of frames.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="frames"></param>
        /// <returns></returns>
        public bool IsUnheldFor(Keys key, int frames)
        {
            return FramesSinceKeyReleased[key] == frames;
        }

        /// <summary>
        /// Check if a combination of keys have been unpressed for exactly the given number of frames.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="frames"></param>
        /// <returns></returns>
        public bool IsUnheldFor(KeyCombination keys, int frames)
        {
            return keys.Keys.All(k => IsUnheldFor(k, frames));
        }

        /// <summary>
        /// Check if a key has been unpressed for at least the given number of frames.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="frames"></param>
        /// <returns></returns>
        public bool IsUnheldForAtleast(Keys key, int frames)
        {
            return FramesSinceKeyReleased[key] >= frames;
        }

        /// <summary>
        /// Check if a combination of keys have been unpressed for at least the given number of frames.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="frames"></param>
        /// <returns></returns>
        public bool IsUnheldForAtleast(KeyCombination keys, int frames)
        {
            return keys.Keys.All(k => IsUnheldForAtleast(k, frames));
        }

        /// <summary>
        /// Check if the keyboard has been idle for exactly the given number of frames.
        /// </summary>
        /// <param name="frames"></param>
        /// <returns></returns>
        public bool IsIdleFor(int frames)
        {
            return FramesSinceLastKeyboardActivity == frames;
        }

        /// <summary>
        /// Check if the keyboard has been idle for at least the given number of frames.
        /// </summary>
        /// <param name="frames"></param>
        /// <returns></returns>
        public bool IsIdleForAtleast(int frames)
        {
            return FramesSinceLastKeyboardActivity >= frames;
        }
    }
}
