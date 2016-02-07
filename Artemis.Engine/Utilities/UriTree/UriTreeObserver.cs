#region Using Statements

using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Utilities.UriTree
{
    
    /// <summary>
    /// A UriTreeObserver is an object that acts as a root element for a
    /// UriTree without actually being a node itself (hence the name, it
    /// "observes" the tree without being a part of it).
    /// </summary>
    public class UriTreeObserver<T> where T : UriTreeNode<T>
    {

        /// <summary>
        /// The dictionary of observed nodes. Note that the nodes in this dictionary
        /// are top level nodes (i.e. one's who's names are just a single part, like
        /// "a" or "b" instead of "a.b").
        /// </summary>
        protected readonly Dictionary<string, T> ObservedNodes = new Dictionary<string, T>();

        private UriTreeObserverMixin<T> observerMixin = new UriTreeObserverMixin<T>();

        /// <summary>
        /// Add an observed node with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="node"></param>
        /// <param name="disallowDuplicates"></param>
        protected void AddObservedNode(string name, T node, bool disallowDuplicates = true)
        {
            observerMixin.AddObservedNode(ObservedNodes, name, node, disallowDuplicates);
        }

        /// <summary>
        /// Insert an observed node with the given name. As with UriTreeNode.InsertNode, this 
        /// will also create any missing intermediate nodes between the deepest existing node
        /// and the given node.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="node"></param>
        /// <param name="disallowDuplicates"></param>
        protected void InsertObservedNode(string name, T node, bool disallowDuplicates = true)
        {
            observerMixin.InsertObservedNode(ObservedNodes, name, node, disallowDuplicates);
        }

        /// <summary>
        /// Return the observed node with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetObservedNode(string name)
        {
            return observerMixin.GetObservedNode(ObservedNodes, name);
        }

    }
}
