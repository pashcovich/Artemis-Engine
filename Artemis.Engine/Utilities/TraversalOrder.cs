using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artemis.Engine.Utilities
{
    /// <summary>
    /// Options indicating how to traverse a traversable object (usually a UriTreeGroup or subtype).
    /// </summary>
    public enum TraversalOptions
    {
        /// <summary>
        /// Visit the subgroups before the items.
        /// </summary>
        Pre,
        /// <summary>
        /// Visit the subgroups after the items.
        /// </summary>
        Post,
        /// <summary>
        /// Only visit the items.
        /// </summary>
        Top
    }
}
