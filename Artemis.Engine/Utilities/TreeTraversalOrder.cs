#region Using Statements

using System;

#endregion

namespace Artemis.Engine.Utilities
{
    public enum TreeTraversalOrder
    {
        /// <summary>
        /// Specifies pre-order traversal for tree traversal algorithms.
        /// </summary>
        Pre,

        /// <summary>
        /// Specifices in-order traversal for tree traversal algorithms.
        /// </summary>
        In,

        /// <summary>
        /// Specifies post-order traversal for tree traversal algorithms.
        /// </summary>
        Post
    }

    /// <summary>
    /// An exception thrown when an algorithm is supplied an invalid TreeTraversalOrder exception.
    /// </summary>
    [Serializable]
    public sealed class TreeTraversalOrderException : Exception
    {
        public TreeTraversalOrderException(TreeTraversalOrder order) : base(
            String.Format("Invalid tree traversal order '{0}'.", order)) { }
    }
}
