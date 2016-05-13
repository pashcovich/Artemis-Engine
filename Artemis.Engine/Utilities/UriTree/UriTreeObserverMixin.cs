#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Artemis.Engine.Utilities.UriTree
{
    /// <summary>
    /// Because MI isn't supported by C#, we can't have UriTreeObserverNode inherit from
    /// both UriTreeObserver and UriTreeNode. Introducing a mixin that contains all the
    /// shared methods for both is the simplest workaround.
    /// </summary>
    /// <typeparam name="U"></typeparam>
    internal class UriTreeObserverMixin<U> where U : UriTreeNode<U>
    {

        public void AddObservedNode(
            Dictionary<string, U> observedNodes, string name, U node, 
            UriTreeNode<U>.UriTreeNodeDelegate onNodeAdded, bool disallowDuplicates = true)
        {
            AddObservedNode(
                observedNodes, new Queue<string>(UriUtilities.GetParts(name)), 
                node, onNodeAdded, disallowDuplicates);
        }

        public void AddObservedNode(
            Dictionary<string, U> observedNodes, Queue<string> nameParts, U node, 
            UriTreeNode<U>.UriTreeNodeDelegate onNodeAdded, bool disallowDuplicates = true)
        {
            var first = nameParts.Dequeue();
            if (nameParts.Count > 0)
            {
                if (!observedNodes.ContainsKey(first))
                {
                    throw new UriTreeException(
                        String.Format(
                            "Could not add node with unqualified name '{0}' to UriTreeObserver. " +
                            "Missing intermediate node with name '{1}'. Consider using InsertObservedNode instead.",
                            String.Join(UriUtilities.URI_SEPARATOR.ToString(), nameParts), first
                            )
                        );
                }
                observedNodes[first].AddSubnode(nameParts, node, disallowDuplicates);
            }
            else
            {
                if (observedNodes.ContainsKey(first))
                {
                    if (disallowDuplicates)
                    {
                        throw new UriTreeException(
                            String.Format(
                                "Could not add node with full name '{0}' to UriTreeObserver. " +
                                "A node with that name already exists.", first
                                )
                           );
                    }
                    observedNodes[first] = node;
                }
                else
                {
                    observedNodes.Add(first, node);
                }
                if (onNodeAdded != null)
                    onNodeAdded(node);
            }
        }

        public void InsertObservedNode(
            Dictionary<string, U> observedNodes, string name, U node, 
            UriTreeNode<U>.UriTreeNodeDelegate onNodeAdded, bool disallowDuplicates = true)
        {
            InsertObservedNode<U>(
                observedNodes, new Queue<string>(UriUtilities.GetParts(name)), 
                node, onNodeAdded, disallowDuplicates);
        }

        public void InsertObservedNode<NodeType>(
            Dictionary<string, U> observedNodes, Queue<string> nameParts, U node,
            UriTreeNode<U>.UriTreeNodeDelegate onNodeAdded, bool disallowDuplicates = true)
            where NodeType : U
        {
            var first = nameParts.Dequeue();
            if (nameParts.Count > 0)
            {
                if (!observedNodes.ContainsKey(first))
                {
                    observedNodes.Add(first, (U)Activator.CreateInstance(typeof(NodeType), first));
                }
                observedNodes[first].InsertSubnode<NodeType>(nameParts, node, disallowDuplicates);
            }
            else
            {
                if (observedNodes.ContainsKey(first))
                {
                    if (disallowDuplicates)
                    {
                        throw new UriTreeException(
                            String.Format(
                                "Could not insert node with full name '{0}' to UriTreeObserver. " +
                                "A node with that name already exists.", first
                                )
                           );
                    }
                    observedNodes[first] = node;
                }
                else
                {
                    observedNodes.Add(first, node);
                }
                if (onNodeAdded != null)
                    onNodeAdded(node);
            }
        }

        public U GetObservedNode(Dictionary<string, U> observedNodes, string name)
        {
            if (name.Contains(UriUtilities.URI_SEPARATOR))
            {
                var firstPart = UriUtilities.GetFirstPart(name);
                var allButFirst = UriUtilities.AllButFirstPart(name);

                return observedNodes[firstPart].GetSubnode(allButFirst);
            }
            return observedNodes[name];
        }
    }
}
