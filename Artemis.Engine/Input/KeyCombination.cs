#region Using Statements

using Microsoft.Xna.Framework.Input;

#endregion

namespace Artemis.Engine.Input
{
    public struct KeyCombination
    {

        /// <summary>
        /// The keys to check in this combination.
        /// </summary>
        public Keys[] Keys;

        public KeyCombination(params Keys[] keys)
        {
            Keys = keys;
        }
    }
}
