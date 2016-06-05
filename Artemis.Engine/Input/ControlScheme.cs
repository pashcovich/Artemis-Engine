#region Using Statements

using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Input
{
    public class ControlScheme
    {
        /// <summary>
        /// Basic control schemes for a top-down game.
        /// </summary>
        public static class TopDown
        {
            public static readonly ControlScheme WASD = new ControlScheme(
                new Dictionary<Keys, ControlIntent> 
                {
                    {Keys.W, ControlIntent.MoveUp},
                    {Keys.A, ControlIntent.MoveLeft},
                    {Keys.S, ControlIntent.MoveDown},
                    {Keys.D, ControlIntent.MoveRight}
                });

            public static readonly ControlScheme Arrows = new ControlScheme(
                new Dictionary<Keys, ControlIntent>
                {
                    {Keys.Up,    ControlIntent.MoveUp},
                    {Keys.Down,  ControlIntent.MoveDown},
                    {Keys.Left,  ControlIntent.MoveLeft},
                    {Keys.Right, ControlIntent.MoveRight}
                });

            public static readonly ControlScheme ArrowsWASD = WASD | Arrows;
        }

        /// <summary>
        /// Basic control schemes for a side-scroller game.
        /// </summary>
        public static class SideScroller
        {
            public static readonly ControlScheme WASD = new ControlScheme(
                new Dictionary<Keys, ControlIntent> 
                {
                    {Keys.W, ControlIntent.Jump},
                    {Keys.A, ControlIntent.MoveLeft},
                    {Keys.S, ControlIntent.Duck},
                    {Keys.D, ControlIntent.MoveRight}
                });

            public static readonly ControlScheme Arrows = new ControlScheme(
                new Dictionary<Keys, ControlIntent>
                {
                    {Keys.Up,    ControlIntent.Jump},
                    {Keys.Down,  ControlIntent.Duck},
                    {Keys.Left,  ControlIntent.MoveLeft},
                    {Keys.Right, ControlIntent.MoveRight}
                });

            public static readonly ControlScheme ArrowsWASD = WASD | Arrows;
        }

        public Dictionary<Keys, ControlIntent> KeyIntents
            = new Dictionary<Keys, ControlIntent>();

        public ControlScheme(Dictionary<Keys, ControlIntent> keys)
        {
            KeyIntents = keys;
        }

        public static ControlScheme operator |(ControlScheme a, ControlScheme b)
        {
            var newKeys = new Dictionary<Keys, ControlIntent>();
            foreach (var kvp in a.KeyIntents)
            {
                newKeys.Add(kvp.Key, kvp.Value);
            }
            foreach (var kvp in b.KeyIntents)
            {
                newKeys.Add(kvp.Key, kvp.Value);
            }
            return new ControlScheme(newKeys);
        }
    }
}
