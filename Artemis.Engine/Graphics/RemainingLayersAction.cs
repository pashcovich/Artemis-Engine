
namespace Artemis.Engine.Graphics
{
    /// <summary>
    /// An enum that describes what to do with the remaining layers not
    /// specified in the render order.
    /// </summary>
    public enum RemainingLayersAction
    {
        /// <summary>
        /// Do nothing with them.
        /// </summary>
        Ignore,

        /// <summary>
        /// Render them before everything else.
        /// </summary>
        RenderFirst,

        /// <summary>
        /// Render them after everything else.
        /// </summary>
        RenderLast
    }
}
