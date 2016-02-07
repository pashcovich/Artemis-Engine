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
            return GetSubnode(UriUtilities.GetParts(fullName), failQuiet);
        }

        internal T GetSubnode(string[] subnodeNameParts, bool failQuiet)
        {
            if (!Subnodes.ContainsKey(subnodeNameParts[0]))
            {
                if (failQuiet)
                {
                    return null;
                }
                throw new UriTreeException(
                    String.Format(
                        "Could not retrieve subnode with unqualified name '{0}' " +
                        "from node with full name '{1}'. No node exists with the given name.", 
                        subnodeNameParts[0], FullName
                        )
                    );
            }
            if (subnodeNameParts.Length == 1)
            {
                return Subnodes[subnodeNameParts[0]];
            }
            var newParts = subnodeNameParts.Skip(1).ToArray();
            return Subnodes[subnodeNameParts[0]].GetSubnode(newParts, failQuiet);
        }

        /// <summary>
        /// Remove a subnode with the given full asset URI (without the
        /// parent node name).
        /// </summary>
        /// <param name="fullName"></param>
        public void RemoveSubnode(string fullName, bool failQuiet = true)
        {
            RemoveSubnode(UriUtilities.GetParts(fullName), failQuiet);
        }

        internal void RemoveSubnode(string[] nameParts, bool failQuiet)
        {
            if (!Subnodes.ContainsKey(nameParts[0]))
            {
                if (failQuiet)
                {
                    return;
                }
                throw new UriTreeException(
                    String.Format(
                        "Could not remove node with unqualified name '{0}' from " +
                        "node with full name '{1}'. No node exists with the given name.",
                        nameParts.Last(), FullName
                        )
                    );
            }

            if (nameParts.Length == 1)
            {
                var name = nameParts[0];

                var asDisposable = Subnodes[name] as IDisposable;
                if (asDisposable != null)
                {
                    asDisposable.Dispose();
                }

                Subnodes.Remove(name);

                return;
            }
            var newParts = nameParts.Skip(1).ToArray();
            Subnodes[nameParts[0]].RemoveSubnode(newParts, failQuiet);
        }

        /// <summary>
        /// Add a subnode to this node with the given full name.
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="subnode"></param>
        /// <param name="disallowDuplicates"></param>
        public void AddSubnode(string fullName, T subnode, bool disallowDuplicates = true)
        {
            AddSubnode(UriUtilities.GetParts(fullName), subnode, disallowDuplicates);
        }

        internal void AddSubnode(string[] nameParts, T subnode, bool disallowDuplicates)
        {
            if (nameParts.Length == 1)
            {
                if (Subnodes.ContainsKey(nameParts[0]))
                {
                    if (disallowDuplicates)
                    {
                        throw new UriTreeException(
                            String.Format(
                                "Could not add subgroup; a group with unqualified name '{0}' " +
                                "already exists in group with full name '{1}'.", nameParts[0], FullName
                                )
                            );
                    }
                    Subnodes[nameParts[0]] = subnode;
                }
                else
                {
                    Subnodes.Add(nameParts[0], subnode);
                }
                subnode.SetParent((T)this);
            }
            else
            {
                var newParts = nameParts.Skip(1).ToArray();
                if (!Subnodes.ContainsKey(nameParts[0]))
                {
                    throw new UriTreeException(
                        String.Format(
                            "Could not add subgroup with full name '{0}' to group with full name '{1}'; " +
                            "intermediate groups missing. Consider using InsertSubnode instead.",
                            nameParts[0], FullName
                            )
                        );
                }
                Subnodes[nameParts[0]].AddSubnode(newParts, subnode, disallowDuplicates);
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
            AddInsertSubnode<NodeType>(UriUtilities.GetParts(fullName), disallowDuplicates);
        }

        internal void AddInsertSubnode<NodeType>(string[] nameParts, bool disallowDuplicates)
            where NodeType : UriTreeNode<T>
        {
            var firstPart = nameParts[0];

            if (nameParts.Length == 1)
            {
                var currentNode = (T)Activator.CreateInstance(typeof(NodeType), firstPart);
                if (Subnodes.ContainsKey(firstPart))
                {
                    if (disallowDuplicates)
                    {
                        throw new UriTreeException(
                            String.Format(
                                "Could not insert subnode, a node with full name '{0}' " +
                                "already exists.", Subnodes[firstPart].FullName
                                )
                            );
                    }
                    Subnodes[firstPart] = currentNode;
                }
                else
                {
                    Subnodes.Add(firstPart, currentNode);
                }
                currentNode.SetParent((T)this);
            }
            else
            {
                if (!Subnodes.ContainsKey(firstPart))
                {
                    var currentNode = (T)Activator.CreateInstance(typeof(NodeType), firstPart);
                    currentNode.SetParent((T)this); // will add to Subnodes automatically.
                }
                var newParts = nameParts.Skip(1).ToArray();
                Subnodes[firstPart].AddInsertSubnode<NodeType>(newParts, disallowDuplicates);
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
            InsertSubnode<NodeType>(UriUtilities.GetParts(fullName), subnode, disallowDuplicates);
        }

        internal void InsertSubnode<NodeType>(string[] nameParts, T subnode, bool disallowDuplicates)
            where NodeType : UriTreeNode<T>
        {
            var firstPart = nameParts[0];

            if (nameParts.Length == 1)
            {
                if (Subnodes.ContainsKey(firstPart))
                {
                    if (disallowDuplicates)
                    {
                        throw new UriTreeException(
                            String.Format(
                                "Could not insert subgroup, a group with full name '{0}' " +
                                "already exists.", Subnodes[firstPart].FullName
                                )
                            );
                    }
                    Subnodes[firstPart] = subnode;
                }
                else
                {
                    Subnodes.Add(firstPart, subnode);
                }
                subnode.SetParent((T)this);
            }
            else
            {
                if (!Subnodes.ContainsKey(firstPart))
                {
                    var newNode = (T)Activator.CreateInstance(typeof(NodeType), firstPart);
                    newNode.SetParent((T)this);
                    Subnodes.Add(firstPart, newNode);
                }
                var newParts = nameParts.Skip(1).ToArray();
                Subnodes[firstPart].InsertSubnode<NodeType>(newParts, subnode, disallowDuplicates);
            }
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
