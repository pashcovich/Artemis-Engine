#region Using Statements

using System;
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
    public class UriTreeNode<T> where T : UriTreeNode<T>
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
            Parent.Subnodes.Add(name, (T)this);
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
                        "from node with full name '{1}'.", subnodeNameParts[0], FullName
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
                        "Could not remove node with unqualified name '{0}' " +
                        "from node with full name '{1}'.", nameParts.Last(), FullName
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
                if (Subnodes.ContainsKey(nameParts[0]) && disallowDuplicates)
                {
                    throw new UriTreeException(
                        String.Format(
                            "Could not add subgroup, a group with unqualified name '{0}' " +
                            "already exists in group with full name '{1}'.", nameParts[0], FullName
                            )
                        );
                }
                Subnodes.Add(nameParts[0], subnode);
                subnode.SetParent((T)this);

                return;
            }
            var newParts = nameParts.Skip(1).ToArray();
            Subnodes[nameParts[0]].AddSubnode(newParts, subnode, disallowDuplicates);
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

            if (Subnodes.ContainsKey(firstPart) && nameParts.Length == 1 && disallowDuplicates)
            {
                throw new UriTreeException(
                        String.Format(
                        "Could not insert subnode, a node with full name '{0}' " +
                        "already exists.", Subnodes[firstPart].FullName
                        )
                    );
            }
            else if (!Subnodes.ContainsKey(firstPart))
            {
                Subnodes.Add(firstPart,
                    (T)Activator.CreateInstance(
                        typeof(NodeType),
                        firstPart
                        )
                    );

                if (nameParts.Length > 1)
                {
                    var newParts = nameParts.Skip(1).ToArray();
                    Subnodes[firstPart].AddInsertSubnode<NodeType>(newParts, disallowDuplicates);
                }
            }            
        }

        /// <summary>
        /// Insert a subgroup into the existing tree structure, creating empty subgroups
        /// wherever they don't exist in the tree (similar to what AddInsertSubgroup does).
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
                if (Subnodes.ContainsKey(firstPart) && disallowDuplicates)
                {
                    throw new UriTreeException(
                            String.Format(
                            "Could not insert subgroup, a group with full name '{0}' " +
                            "already exists.", Subnodes[firstPart].FullName
                           )
                        );
                }

                Subnodes[firstPart] = subnode;
                subnode.SetParent((T)this);

                return;
            }

            if (!Subnodes.ContainsKey(firstPart))
            {
                Subnodes.Add(firstPart,
                    (T)Activator.CreateInstance(
                        typeof(NodeType),
                        firstPart
                        )
                    );
            }

            var newParts = nameParts.Skip(1).ToArray();
            Subnodes[firstPart].InsertSubnode<NodeType>(newParts, subnode, disallowDuplicates);
        }
    }
}
