#region Using Statements

using Artemis.Engine;

using System;

#endregion

namespace Artemis.ApprovalTestTemplates.BasicTemplate
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ArtemisEngine.Setup("game.setup", Initialize);
        }

        static void Initialize()
        {
            
        }
    }
#endif
}
