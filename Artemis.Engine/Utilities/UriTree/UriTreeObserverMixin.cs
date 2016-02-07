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
            Dictionary<string, U> observedNodes, string name, U node, bool disallowDuplicates = true)
        {
            if (name.Contains(UriUtilities.URI_SEPARATOR))
            {
                var firstPart = UriUtilities.GetFirstPart(name);
                var allButFirst = UriUtilities.AllButFirstPart(name);

                if (!observedNodes.ContainsKey(firstPart))
                {
                    throw new UriTreeException(
                        String.Format(
                            "Could not add node with unqualified name '{0}' to UriTreeObserver. " +
                            "Missing intermediate node with name '{1}'. Consider using InsertObservedNode instead.",
                            name, firstPart
                            )
                        );
                }
                observedNodes[firstPart].AddSubnode(allButFirst, node, disallowDuplicates);
            }
            else
            {
                if (observedNodes.ContainsKey(name))
                {
                    if (disallowDuplicates)
                    {
                        throw new UriTreeException(
                            String.Format(
                                "Could not add node with full name '{0}' to UriTreeObserver. " +
                                "A node with that name already exists.", name
                                )
                           );
                    }
                    observedNodes[name] = node;
                }
                else
                {
                    observedNodes.Add(name, node);
                }
            }
        }

        public void InsertObservedNode(
            Dictionary<string, U> observedNodes, string name, U node, bool disallowDuplicates = true)
        {
            InsertObservedNode<U>(observedNodes, name, node, disallowDuplicates);
        }

        public void InsertObservedNode<NodeType>(
            Dictionary<string, U> observedNodes, string name, U node, bool disallowDuplicates = true)
            where NodeType : U
        {
            if (name.Contains(UriUtilities.URI_SEPARATOR))
            {
                var firstPart = UriUtilities.GetFirstPart(name);
                var allButFirst = UriUtilities.AllButFirstPart(name);

                if (!observedNodes.ContainsKey(firstPart))
                {
                    observedNodes.Add(firstPart, (U)Activator.CreateInstance(typeof(NodeType), firstPart));
                }
                observedNodes[firstPart].InsertSubnode<NodeType>(allButFirst, node, disallowDuplicates);
            }
            else
            {
                if (observedNodes.ContainsKey(name))
                {
                    if (disallowDuplicates)
                    {
                        throw new UriTreeException(
                            String.Format(
                                "Could not insert node with full name '{0}' to UriTreeObserver. " +
                                "A node with that name already exists.", name
                                )
                           );
                    }
                    observedNodes[name] = node;
                }
                else
                {
                    observedNodes.Add(name, node);
                }
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
