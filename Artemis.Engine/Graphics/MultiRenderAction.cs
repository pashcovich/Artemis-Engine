
namespace Artemis.Engine.Graphics
{
    /// <summary>
    /// An enum that describes what to do when a layer is accidentally (or intentionlly)
    /// rendered multiple times.
    /// </summary>
    public enum MultiRenderAction
    {
        /// <summary>
        /// Do nothing with it.
        /// </summary>
        Ignore,

        /// <summary>
        /// Render it again.
        /// </summary>
        RenderAgain,

        /// <summary>
        /// Log the event with level "Warn".
        /// </summary>
        LogWarn,

        /// <summary>
        /// Log the event with level "Critical".
        /// </summary>
        LogCritical,

        /// <summary>
        /// Throw an exception.
        /// </summary>
        Fail
    }
}
