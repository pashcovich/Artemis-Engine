#region Using Statements

using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Input
{
    /// <summary>
    /// Represents the intent of a controller input in a ControlScheme.
    /// 
    /// This class uses the Flyweight pattern. You create instances by calling
    /// ControlIntent.GetIntent(name). If an intent instance already exists with
    /// the given name, it is returned, otherwise a new one is created and returned.
    /// </summary>
    public sealed class ControlIntent
    {
        /// <summary>
        /// The name of this intent.
        /// </summary>
        public string Name;
        private ControlIntent(string name) { Name = name; }

        /// <summary>
        /// The dictionary of Intent objects for the Flyweight implementation.
        /// </summary>
        private static Dictionary<string, ControlIntent> Intents
            = new Dictionary<string, ControlIntent>();

        static ControlIntent()
        {
            Intents.Add("MoveLeft",  new ControlIntent("MoveLeft"));
            Intents.Add("MoveRight", new ControlIntent("MoveRight"));
            Intents.Add("MoveUp",    new ControlIntent("MoveUp"));
            Intents.Add("MoveDown",  new ControlIntent("MoveDown"));
            Intents.Add("Jump",      new ControlIntent("Jump"));
            Intents.Add("Duck",      new ControlIntent("Duck"));
            Intents.Add("Exit",      new ControlIntent("Exit"));
        }

        public static readonly ControlIntent MoveLeft  = Intents["MoveLeft"];
        public static readonly ControlIntent MoveRight = Intents["MoveRight"];
        public static readonly ControlIntent MoveUp    = Intents["MoveUp"];
        public static readonly ControlIntent MoveDown  = Intents["MoveDown"];
        public static readonly ControlIntent Jump      = Intents["Jump"];
        public static readonly ControlIntent Duck      = Intents["Duck"];
        public static readonly ControlIntent Exit      = Intents["Exit"];

        /// <summary>
        /// Get the intent with the given name if it exists, otherwise create it and return it.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ControlIntent GetIntent(string name)
        {
            if (Intents.ContainsKey(name))
                return Intents[name];

            var intent = new ControlIntent(name);
            Intents.Add(name, intent);

            return intent;
        }
    }
}
