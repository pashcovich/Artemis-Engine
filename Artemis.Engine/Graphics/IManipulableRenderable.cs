#region Using Statements

using Microsoft.Xna.Framework;

#endregion

namespace Artemis.Engine.Graphics
{

    /// <summary>
    /// Represents a Renderable whose properties can be manipulated by the layer it
    /// belongs to.
    /// </summary>
    public interface IManipulableRenderable : IRenderable
    {
        RelativePosition Position { get; set; }

        RenderComponents Components { get; set; }

        ResolutionScaleRules ResolutionScaleRules { get; set; }

    }
}
