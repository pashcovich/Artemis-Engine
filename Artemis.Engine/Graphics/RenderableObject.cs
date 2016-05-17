#region Using Statements

using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Graphics
{

    /// <summary>
    /// A delegate fired when the LayerTarget of a Layer changes.
    /// </summary>
    /// <param name="previousTarget"></param>
    /// <param name="currentTarget"></param>
    public delegate void LayerTargetChangedDelegate(RenderTarget2D previousTarget, RenderTarget2D currentTarget);

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

        /// <summary>
        /// The event fired when the target of the layer is changed.
        /// </summary>
        public LayerTargetChangedDelegate OnLayerTargetChanged;

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

        public RenderableObject() : base() { }

        public override void Kill()
        {
            base.Kill();

            Layer.targetChangeListeners.Remove(this);
        }
    }
}
