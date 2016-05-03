#region Using Statements

using Artemis.Engine;

using System;

#endregion

namespace Artemis.ApprovalTestTemplates.MultiformTemplate
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
            ArtemisEngine.Setup("game.constants", Setup);
            ArtemisEngine.Begin(Initialize);
        }

        static void Setup()
        { 

        }

        static void Initialize()
        {
            ArtemisEngine.RegisterMultiforms(typeof(MultiformTemplate));

            /* Alternative Ways of registering the MultiformTemplate:
             * 
             * ArtemisEngine.RegisterMultiforms(new MultiformTemplate("name"));
             * 
             * ArtemisEngine.RegisterMultiforms(new MultiformTemplate());
             * 
             * The user can also decorate the MultiformTemplate class with a 
             * NamedMultiformAttribute.
             */

            ArtemisEngine.StartWith("MultiformTemplate");
        }
    }
#endif
}
