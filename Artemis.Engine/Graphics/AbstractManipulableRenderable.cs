using Microsoft.Xna.Framework;

namespace Artemis.Engine.Graphics
{

    /// <summary>
    /// Represents a Renderable whose properties can be manipulated by the layer it
    /// belongs to.
    /// </summary>
    public abstract class AbstractManipulableRenderable : AbstractRenderable
    {
        public RelativePosition Position;

        public RenderComponents Components;

        public ResolutionScaleRules ResolutionScaleRules;

        internal void Render(
            RelativePosition manipulatedPosition, RenderComponents manipulatedComponents)
        {
            var oldPos = Position;
            var oldComps = Components;

            Position = manipulatedPosition;
            Components = manipulatedComponents;

            Render();

            Position = oldPos;
            Components = oldComps;
        }
    }
}
