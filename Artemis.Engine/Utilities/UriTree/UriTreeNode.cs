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

        /// <summary>
        /// The unqualified name of this group.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The full name of this group all the way up the asset group tree.
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
        /// The parent of this group (if it exists).
        /// </summary>
        public T Parent { get; private set; }

        /// <summary>
        /// Check if this group is a leaf node (i.e. has no children).
        /// </summary>
        public bool IsLeaf
        {
            get
            {
                return (Subgroups.Count == 0);
            }
        }

        /// <summary>
        /// The dictionary of subgroups of this group, mapping from unqualified 
        /// names to associated subgroups.
        /// </summary>
        public Dictionary<string, T> Subgroups { get; protected set; }

        protected UriTreeNode(string name)
        {
            Name = name;
            Subgroups = new Dictionary<string, T>();
        }

        protected void SetParent(T parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Return a subgroup of this group with the given full name.
        /// 
        /// Example:
        /// If this group's name is "Parent", then calling GetSubgroup("Child.GrandChild")
        /// will return the group "Parent.Child.GrandChild".
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public T GetSubgroup(string fullName, bool failQuiet = false)
        {
            return GetSubgroup(UriUtilities.GetParts(fullName), failQuiet);
        }

        internal T GetSubgroup(string[] subgroupNameParts, bool failQuiet)
        {
            if (!Subgroups.ContainsKey(subgroupNameParts[0]))
            {
                if (failQuiet)
                {
                    return null;
                }
                throw new UriTreeException(
                    String.Format(
                        "Could not retrieve subgroup with unqualified name '{0}' " +
                        "from group  with full name '{1}'.", subgroupNameParts[0], FullName
                        )
                    );
            }
            if (subgroupNameParts.Length == 1)
            {
                return Subgroups[subgroupNameParts[0]];
            }
            var newParts = subgroupNameParts.Skip(1).ToArray();
            return Subgroups[subgroupNameParts[0]].GetSubgroup(newParts, failQuiet);
        }

        /// <summary>
        /// Remove a subgroup with the given full asset URI (without the
        /// parent group name).
        /// </summary>
        /// <param name="fullName"></param>
        public void RemoveSubgroup(string fullName, bool failQuiet = true)
        {
            RemoveSubgroup(UriUtilities.GetParts(fullName), failQuiet);
        }

        internal void RemoveSubgroup(string[] nameParts, bool failQuiet)
        {
            if (!Subgroups.ContainsKey(nameParts[0]))
            {
                if (failQuiet)
                {
                    return;
                }
                throw new UriTreeException(
                    String.Format(
                        "Could not remove group with unqualified name '{0}' " +
                        "from group with full name '{1}'.", nameParts.Last(), FullName
                        )
                    );
            }

            if (nameParts.Length == 1)
            {
                var name = nameParts[0];

                var asDisposable = Subgroups[name] as IDisposable;
                if (asDisposable != null)
                {
                    asDisposable.Dispose();
                }

                Subgroups.Remove(name);

                return;
            }
            var newParts = nameParts.Skip(1).ToArray();
            Subgroups[nameParts[0]].RemoveSubgroup(newParts, failQuiet);
        }

        /// <summary>
        /// Add a subgroup to this group with the given full name.
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="subgroup"></param>
        /// <param name="disallowDuplicates"></param>
        public void AddSubgroup(string fullName, T subgroup, bool disallowDuplicates = true)
        {
            AddSubgroup(UriUtilities.GetParts(fullName), subgroup, disallowDuplicates);
        }

        internal void AddSubgroup(string[] nameParts, T subgroup, bool disallowDuplicates)
        {
            if (nameParts.Length == 1)
            {
                if (Subgroups.ContainsKey(nameParts[0]) && disallowDuplicates)
                {
                    throw new UriTreeException(
                        String.Format(
                            "Could not add subgroup, a group with unqualified name '{0}' " +
                            "already exists in group with full name '{1}'.", nameParts[0], FullName
                            )
                        );
                }
                Subgroups.Add(nameParts[0], subgroup);
                subgroup.SetParent((T)this);

                return;
            }
            var newParts = nameParts.Skip(1).ToArray();
            Subgroups[nameParts[0]].AddSubgroup(newParts, subgroup, disallowDuplicates);
        }

        /// <summary>
        /// Create and insert a subgroup with the given full name into this group.
        /// 
        /// Inserting a subgroup is different from simply adding a subgroup in that
        /// if there are any "missing" subgroups between the lowest existent subgroup
        /// and the given subgroup, they will be added as instances of the given NodeType
        /// generic parameter.
        /// 
        /// For example, if we are attempting to add "a.b.c.d.e.f" to a group who's deepest
        /// subgroup is "a.b.c", then groups "a.b.c.d", and "a.b.c.d.e" will be created as
        /// well as "a.b.c.d.e.f".
        /// </summary>
        /// <typeparam name="NodeType"></typeparam>
        /// <param name="fullName"></param>
        /// <param name="disallowDuplicates"></param>
        public void AddInsertSubgroup<NodeType>(string fullName, bool disallowDuplicates = true)
            where NodeType : UriTreeNode<T>
        {
            AddInsertSubgroup<NodeType>(UriUtilities.GetParts(fullName), disallowDuplicates);
        }

        internal void AddInsertSubgroup<NodeType>(string[] nameParts, bool disallowDuplicates)
            where NodeType : UriTreeNode<T>
        {
            var firstPart = nameParts[0];

            if (Subgroups.ContainsKey(firstPart) && nameParts.Length == 1 && disallowDuplicates)
            {
                throw new UriTreeException(
                        String.Format(
                        "Could not insert subgroup, a group with full name '{0}' " +
                        "already exists.", Subgroups[firstPart].FullName
                        )
                    );
            }
            else if (!Subgroups.ContainsKey(firstPart))
            {
                Subgroups.Add(firstPart,
                    (T)Activator.CreateInstance(
                        typeof(NodeType),
                        firstPart
                        )
                    );

                if (nameParts.Length > 1)
                {
                    var newParts = nameParts.Skip(1).ToArray();
                    Subgroups[firstPart].AddInsertSubgroup<NodeType>(newParts, disallowDuplicates);
                }
            }            
        }

        /// <summary>
        /// Insert a subgroup into the existing tree structure, creating empty subgroups
        /// wherever they don't exist in the tree (similar to what AddInsertSubgroup does).
        /// </summary>
        /// <typeparam name="NodeType"></typeparam>
        /// <param name="fullName"></param>
        /// <param name="subgroup"></param>
        /// <param name="disallowDuplicates"></param>
        public void InsertSubgroup<NodeType>(string fullName, T subgroup, bool disallowDuplicates = true)
            where NodeType : UriTreeNode<T>
        {
            InsertSubgroup<NodeType>(UriUtilities.GetParts(fullName), subgroup, disallowDuplicates);
        }

        internal void InsertSubgroup<NodeType>(string[] nameParts, T subgroup, bool disallowDuplicates)
            where NodeType : UriTreeNode<T>
        {
            var firstPart = nameParts[0];

            if (nameParts.Length == 1)
            {
                if (Subgroups.ContainsKey(firstPart) && disallowDuplicates)
                {
                    throw new UriTreeException(
                            String.Format(
                            "Could not insert subgroup, a group with full name '{0}' " +
                            "already exists.", Subgroups[firstPart].FullName
                           )
                        );
                }

                Subgroups[firstPart] = subgroup;
                subgroup.SetParent((T)this);

                return;
            }

            if (!Subgroups.ContainsKey(firstPart))
            {
                Subgroups.Add(firstPart,
                    (T)Activator.CreateInstance(
                        typeof(NodeType),
                        firstPart
                        )
                    );
            }

            var newParts = nameParts.Skip(1).ToArray();
            Subgroups[firstPart].InsertSubgroup<NodeType>(newParts, subgroup, disallowDuplicates);
        }
    }
}
