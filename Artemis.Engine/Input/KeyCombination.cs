#region Using Statements

using Microsoft.Xna.Framework.Input;

#endregion

namespace Artemis.Engine.Input
{
    public class KeyCombination
    {
        public Keys[] Keys { get; private set; }

        public KeyCombination(params Keys[] keys)
        {
            Keys = keys;
        }
    }
}
