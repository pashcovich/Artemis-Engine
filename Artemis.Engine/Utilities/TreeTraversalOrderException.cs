#region Using Statements

using System;

#endregion

namespace Artemis.Engine.Utilities
{
    /// <summary>
    /// An exception thrown when an algorithm is supplied an invalid TreeTraversalOrder exception.
    /// </summary>
    [Serializable]
    public sealed class TreeTraversalOrderException : Exception
    {
        public TreeTraversalOrderException(TreeTraversalOrder order)
            : base(
                String.Format("Invalid tree traversal order '{0}'.", order)) { }
    }
}
