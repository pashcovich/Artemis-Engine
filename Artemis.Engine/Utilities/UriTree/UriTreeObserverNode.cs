#region Using Statements

using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Utilities.UriTree
{

    /// <summary>
    /// A UriTreeObserverNode is simultaneously a Node in a UriTree, but also "observes" 
    /// another UriTree (i.e. it acts as a root element for a separate tree). It is a
    /// UriTreeObserver that is also a UriTreeNode.
    /// 
    /// Here "T" must be the class itself, whereas "U" must be a subclass of UriTreeNode&lt;U&gt;
    /// that represents the separate nodes being observed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public class UriTreeObserverNode<T, U> : UriTreeNode<T> where U : UriTreeNode<U>
    {

        /// <summary>
        /// The dictionary of observed nodes. Note that the nodes in this dictionary
        /// are top level nodes (i.e. one's who's names are just a single part, like
        /// "a" or "b" instead of "a.b").
        /// </summary>
        protected readonly Dictionary<string, U> ObservedNodes = new Dictionary<string, U>();

        private UriTreeObserverMixin<U> observerMixin = new UriTreeObserverMixin<U>();

        public UriTreeObserverNode(string name) : base(name) { }

        /// <summary>
        /// Add an observed node with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="node"></param>
        /// <param name="disallowDuplicates"></param>
        protected void AddObservedNode(string name, U node, bool disallowDuplicates = true)
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
        protected void InsertObservedNode(string name, U node, bool disallowDuplicates = true)
        {
            observerMixin.InserObservedtNode(ObservedNodes, name, node, disallowDuplicates);
        }

        /// <summary>
        /// Return the observed node with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected U GetObservedNode(string name)
        {
            return observerMixin.GetObservedNode(ObservedNodes, name);
        }

    }
}
