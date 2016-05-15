#region Using Statements

using System;
using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Graphics
{
    public abstract class RenderableObject : ResolutionRelativeObject
    {
        /// <summary>
        /// The Layer this object belongs to.
        /// </summary>
        public AbstractRenderLayer Layer { get; internal set; }

        /// <summary>
        /// Whether or not this object is visible.
        /// </summary>
        public bool Visible;

        /// <summary>
        /// Whether or not this object can be safely rendered multiple times in a single game tick.
        /// </summary>
        public bool DisallowMultipleRenders;

        /// <summary>
        /// The components that specify how this object is to be rendered.
        /// </summary>
        public RenderComponents RenderComponents;

        /// <summary>
        /// An abstract method for rendering this object.
        /// </summary>
        public abstract void Render();

        internal void InternalRender(HashSet<RenderableObject> seenObjects)
        {
            if (Visible)
            {
                if (DisallowMultipleRenders && seenObjects.Contains(this))
                {
                    throw new RenderOrderException(
                        String.Format(
                            "Renderable object '{0}' was rendered multiple times. If this is desired behaviour, " +
                            "change the object's `DisallowMultipleRenders` property to false.", this));
                }
                Render();
                seenObjects.Add(this);
            }
        }
    }
}
