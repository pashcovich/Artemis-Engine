#region Using Statements

using Artemis.Engine.Utilities;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Artemis.Engine.Utilities.UriTree
{
    /// <summary>
    /// A recursive container structure where items and subgroups are
    /// accessed using a "URI" (a name with parts separated by the
    /// URI_SEPARATOR character).
    /// </summary>
    /// <typeparam name="T">This must be the class itself, otherwise recursion won't work.</typeparam>
    public class UriTreeNode<T> : IEnumerable<T> where T : UriTreeNode<T>
    {

        public delegate void UriTreeNodeDelegate(UriTreeNode<T> node);

        private string name;

        /// <summary>
        /// The unqualified name of this node. If the fullname is "a.b.c" then the
        /// unqualified name is "c".
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                var prevName = name;
                name = value;
                if (Parent == null)
                    return;

                Parent.Subnodes.Remove(prevName);
                Parent.Subnodes.Add(name, (T)this);
            }
        }

        /// <summary>
        /// The full name of this node all the way up the tree.
        /// </summary>
        public string FullName
        {
            get
            {
                return Parent == null ? Name
                                      : Parent.FullName + UriUtilities.URI_SEPARATOR + Name;
            }
        }

        /// <summary>
        /// The parent of this node (if it exists).
        /// </summary>
        public T Parent { get; private set; }

        /// <summary>
        /// Check if this node is a leaf node (i.e. has no children).
        /// </summary>
        public bool IsLeaf
        {
            get
            {
                return (Subnodes.Count == 0);
            }
        }

        /// <summary>
        /// The dictionary of subnodes of this node, mapping from unqualified 
        /// names to associated subnodes.
        /// </summary>
        public Dictionary<string, T> Subnodes { get; protected set; }

        public UriTreeNodeDelegate OnNodeAdded;
        public UriTreeNodeDelegate OnNodeRemoved;

        protected UriTreeNode(string name)
        {
            Name = name;
            Subnodes = new Dictionary<string, T>();
        }

        public void SetParent(T parent)
        {
            if (Parent != null)
            {
                Parent.Subnodes.Remove(name);
            }

            Parent = parent;
            if (!Parent.Subnodes.ContainsKey(name))
            {
                Parent.Subnodes.Add(name, (T)this);

                if (Parent.OnNodeAdded != null)
                    Parent.OnNodeAdded(this);
            }
        }

        /// <summary>
        /// Return a subnode of this node with the given full name.
        /// 
        /// Example:
        /// If this node's name is "Parent", then calling GetSubnode("Child.GrandChild")
        /// will return the node "Parent.Child.GrandChild".
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public T GetSubnode(string fullName, bool failQuiet = false)
        {
            return GetSubnode(new Queue<string>(UriUtilities.GetParts(fullName)), failQuiet);
        }

        internal T GetSubnode(Queue<string> subnodeNameParts, bool failQuiet)
        {
            var first = subnodeNameParts.Dequeue();
            if (!Subnodes.ContainsKey(first))
            {
                if (failQuiet)
                {
                    return null;
                }
                throw new UriTreeException(
                    String.Format(
                        "Could not retrieve subnode with unqualified name '{0}' " +
                        "from node with full name '{1}'. No node exists with the given name.", 
                        first, FullName
                        )
                    );
            }
            if (subnodeNameParts.Count == 0)
            {
                return Subnodes[first];
            }
            return Subnodes[first].GetSubnode(subnodeNameParts, failQuiet);
        }

        /// <summary>
        /// Remove a subnode with the given full asset URI (without the
        /// parent node name).
        /// </summary>
        /// <param name="fullName"></param>
        public void RemoveSubnode(string fullName, bool failQuiet = true)
        {
            RemoveSubnode(new Queue<string>(UriUtilities.GetParts(fullName)), failQuiet);
        }

        internal void RemoveSubnode(Queue<string> nameParts, bool failQuiet)
        {
            var first = nameParts.Dequeue();
            if (!Subnodes.ContainsKey(first))
            {
                if (failQuiet)
                {
                    return;
                }
                var last = nameParts.Count > 0 ? nameParts.Last() : first;
                throw new UriTreeException(
                    String.Format(
                        "Could not remove node with unqualified name '{0}' from " +
                        "node with full name '{1}'. No node exists with the given name.",
                        last, FullName
                        )
                    );
            }

            if (nameParts.Count == 1)
            {
                var node = Subnodes[first];
                var asDisposable = node as IDisposable;
                if (asDisposable != null)
                {
                    asDisposable.Dispose();
                }

                Subnodes.Remove(first);

                if (OnNodeRemoved != null)
                    OnNodeRemoved(node);

                return;
            }
            Subnodes[first].RemoveSubnode(nameParts, failQuiet);
        }

        /// <summary>
        /// Add a subnode to this node with the given full name.
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="subnode"></param>
        /// <param name="disallowDuplicates"></param>
        public void AddSubnode(string fullName, T subnode, bool disallowDuplicates = true)
        {
            AddSubnode(new Queue<string>(UriUtilities.GetParts(fullName)), subnode, disallowDuplicates);
        }

        internal void AddSubnode(Queue<string> nameParts, T subnode, bool disallowDuplicates)
        {
            var first = nameParts.Dequeue();
            if (nameParts.Count == 0)
            {
                if (Subnodes.ContainsKey(first))
                {
                    if (disallowDuplicates)
                    {
                        throw new UriTreeException(
                            String.Format(
                                "Could not add subgroup; a group with unqualified name '{0}' " +
                                "already exists in group with full name '{1}'.", first, FullName
                                )
                            );
                    }
                    Subnodes[first] = subnode;
                }
                else
                {
                    Subnodes.Add(first, subnode);
                }
                subnode.SetParent((T)this);
            }
            else
            {
                if (!Subnodes.ContainsKey(first))
                {
                    throw new UriTreeException(
                        String.Format(
                            "Could not add subgroup with full name '{0}' to group with full name '{1}'; " +
                            "intermediate groups missing. Consider using InsertSubnode instead.",
                            first, FullName
                            )
                        );
                }
                Subnodes[first].AddSubnode(nameParts, subnode, disallowDuplicates);
            }
        }

        /// <summary>
        /// Create and insert a subnode with the given full name into this group.
        /// 
        /// Inserting a subnode is different from simply adding a subnode in that
        /// if there are any "missing" subnodes between the lowest existent subnode
        /// and the given subnode, they will be added as instances of T.
        /// 
        /// For example, if we are attempting to add "a.b.c.d.e.f" to a node who's deepest
        /// subnode is "a.b.c", then nodes "a.b.c.d", and "a.b.c.d.e" will be created as
        /// well as "a.b.c.d.e.f".
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="disallowDuplicates"></param>
        public void AddInsertSubnode(string fullName, bool disallowDuplicates = true)
        {
            AddInsertSubnode<T>(fullName, disallowDuplicates);
        }

        /// <summary>
        /// Create and insert a subnode with the given full name into this group.
        /// 
        /// Inserting a subnode is different from simply adding a subnode in that
        /// if there are any "missing" subnodes between the lowest existent subnode
        /// and the given subnode, they will be added as instances of the given NodeType
        /// generic parameter.
        /// 
        /// For example, if we are attempting to add "a.b.c.d.e.f" to a node who's deepest
        /// subnode is "a.b.c", then nodes "a.b.c.d", and "a.b.c.d.e" will be created as
        /// well as "a.b.c.d.e.f".
        /// 
        /// Note: for this method to work the supplied NodeType class must have a constructor
        /// that takes a single string argument and calls base(string).
        /// </summary>
        /// <typeparam name="NodeType"></typeparam>
        /// <param name="fullName"></param>
        /// <param name="disallowDuplicates"></param>
        public void AddInsertSubnode<NodeType>(string fullName, bool disallowDuplicates = true)
            where NodeType : UriTreeNode<T>
        {
            AddInsertSubnode<NodeType>(new Queue<string>(UriUtilities.GetParts(fullName)), disallowDuplicates);
        }

        internal void AddInsertSubnode<NodeType>(Queue<string> nameParts, bool disallowDuplicates)
            where NodeType : UriTreeNode<T>
        {
            var first = nameParts.Dequeue();

            if (nameParts.Count == 0)
            {
                var currentNode = (T)Activator.CreateInstance(typeof(NodeType), first);
                if (Subnodes.ContainsKey(first))
                {
                    if (disallowDuplicates)
                    {
                        throw new UriTreeException(
                            String.Format(
                                "Could not insert subnode, a node with full name '{0}' " +
                                "already exists.", Subnodes[first].FullName
                                )
                            );
                    }
                    Subnodes[first] = currentNode;
                }
                else
                {
                    Subnodes.Add(first, currentNode);
                }
                currentNode.SetParent((T)this);
            }
            else
            {
                if (!Subnodes.ContainsKey(first))
                {
                    var currentNode = (T)Activator.CreateInstance(typeof(NodeType), first);
                    currentNode.SetParent((T)this); // will add to Subnodes automatically.
                }
                Subnodes[first].AddInsertSubnode<NodeType>(nameParts, disallowDuplicates);
            }            
        }

        /// <summary>
        /// Insert a subgroup into the existing tree structure, creating empty subgroups
        /// wherever they don't exist in the tree (similar to what AddInsertSubgroup does).
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="subnode"></param>
        /// <param name="disallowDuplicates"></param>
        public void InsertSubnode(string fullName, T subnode, bool disallowDuplicates = true)
        {
            InsertSubnode<T>(fullName, subnode, disallowDuplicates);
        }

        /// <summary>
        /// Insert a subgroup into the existing tree structure, creating empty subgroups
        /// wherever they don't exist in the tree (similar to what AddInsertSubgroup does).
        /// 
        /// Note: for this method to work the supplied NodeType class must have a constructor
        /// that takes a single string argument and calls base(string).
        /// </summary>
        /// <typeparam name="NodeType"></typeparam>
        /// <param name="fullName"></param>
        /// <param name="subnode"></param>
        /// <param name="disallowDuplicates"></param>
        public void InsertSubnode<NodeType>(string fullName, T subnode, bool disallowDuplicates = true)
            where NodeType : UriTreeNode<T>
        {
            InsertSubnode<NodeType>(new Queue<string>(UriUtilities.GetParts(fullName)), subnode, disallowDuplicates);
        }

        internal void InsertSubnode<NodeType>(Queue<string> nameParts, T subnode, bool disallowDuplicates)
            where NodeType : UriTreeNode<T>
        {
            var first = nameParts.Dequeue();

            if (nameParts.Count == 0)
            {
                if (Subnodes.ContainsKey(first))
                {
                    if (disallowDuplicates)
                    {
                        throw new UriTreeException(
                            String.Format(
                                "Could not insert subgroup, a group with full name '{0}' " +
                                "already exists.", Subnodes[first].FullName
                                )
                            );
                    }
                    Subnodes[first] = subnode;
                }
                else
                {
                    Subnodes.Add(first, subnode);
                }
                subnode.SetParent((T)this);
            }
            else
            {
                if (!Subnodes.ContainsKey(first))
                {
                    var newNode = (T)Activator.CreateInstance(typeof(NodeType), first);
                    newNode.SetParent((T)this);
                }
                Subnodes[first].InsertSubnode<NodeType>(nameParts, subnode, disallowDuplicates);
            }
        }

        /// <summary>
        /// Check if this node is the parent of a given node.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsSubnode(string name)
        {
            return ContainsSubnode(new Queue<string>(UriUtilities.GetParts(name)));
        }

        internal bool ContainsSubnode(Queue<string> nameParts)
        {
            if (nameParts.Count == 0)
                return true;
            var first = nameParts.Dequeue();
            if (!Subnodes.ContainsKey(first))
                return false;
            return Subnodes[first].ContainsSubnode(nameParts);
        }

        /// <summary>
        /// Check if this node contains the given subnode.
        /// </summary>
        /// <param name="subnode"></param>
        /// <param name="searchRecursive"></param>
        /// <returns></returns>
        public bool ContainsSubnode(T subnode)
        {
            var fullName = FullName;
            var relativeName = new String(subnode.FullName.SkipWhile((c, i) => c == fullName[i]).ToArray());
            return ContainsSubnode(relativeName);
        }

        public IEnumerator<string> IterateNodeNames()
        {
            foreach (var kvp in Subnodes)
            {
                yield return kvp.Key;
            }
        }

        // TODO: Implement this.

        /*
        public IEnumerator<T> IterateAll(TreeTraversalOrder order)
        {
            switch (order)
            {
                case TreeTraversalOrder.Pre:
                    return 
                    break;
                case TreeTraversalOrder.In:
                    break;
                case TreeTraversalOrder.Post:
                    break;
                default:
                    throw new TreeTraversalOrderException(order);
            }
        }
         */

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var kvp in Subnodes)
            {
                yield return kvp.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }
    }
}
