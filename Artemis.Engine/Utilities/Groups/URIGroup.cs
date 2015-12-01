using System;
using System.Collections.Generic;
using System.Linq;

namespace Artemis.Engine.Utilities.Groups
{

    /// <summary>
    /// A recursive container structure where items and subgroups are
    /// accessed using a "URI" (a name with parts separated by the
    /// URI_SEPARATOR character).
    /// </summary>
    /// <typeparam name="T">This must be the class itself, otherwise recursion won't work.</typeparam>
    /// <typeparam name="U">The type of object stored in each group.</typeparam>
    public class UriGroup<T, U> where T : UriGroup<T, U>
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
        /// Check if this group is empty (i.e. it has no items and it has no
        /// subgroups).
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (Items.Count == 0 && IsLeaf);
            }
        }

        /// <summary>
        /// The dictionary of subgroups of this group, mapping from unqualified 
        /// names to associated subgroups.
        /// </summary>
        protected Dictionary<string, T> Subgroups = new Dictionary<string, T>();

        /// <summary>
        /// The dictionary of all items in this group.
        /// </summary>
        protected Dictionary<string, U> Items = new Dictionary<string, U>();

        protected UriGroup(string name)
        {
            Name = name;
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
                throw new UriGroupException(
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
        /// Return the item with the given full name.
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public U GetItem(string fullName, bool useDefault = false)
        {
            return GetItem(UriUtilities.GetParts(fullName), useDefault);
        }

        internal U GetItem(string[] nameParts, bool useDefault)
        {
            if (nameParts.Length > 1 && !Subgroups.ContainsKey(nameParts[0]))
            {
                if (useDefault)
                {
                    return default(U);
                }
                throw CouldNotRetrieveItem(nameParts[0]);
            }
            else if (nameParts.Length == 1)
            {
                if (!Items.ContainsKey(nameParts[0]))
                {
                    if (useDefault)
                    {
                        return default(U);
                    }
                    throw CouldNotRetrieveItem(nameParts[0]);
                }
                return Items[nameParts[0]];
            }
            var newParts = nameParts.Skip(1).ToArray();
            return Subgroups[nameParts[0]].GetItem(newParts, useDefault);
        }

        /// <summary>
        /// Common exception thrown when an item could not be retrieved.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private UriGroupException CouldNotRetrieveItem(string name)
        {
            return new UriGroupException(
                    String.Format(
                        "Could not retrieve item with name '{0}' " +
                        "from group with full name '{1}'", name, FullName
                        )
                    );
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
                throw new UriGroupException(
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
                if (Subgroups.ContainsKey(nameParts[0]))
                {
                    throw new UriGroupException(
                        String.Format(
                            "Could not add subgroup, a group with unqualified name '{0}' " +
                            "already exists in group with full name '{1}'.", nameParts[0], FullName
                            )
                        );
                }
                Subgroups.Add(nameParts[0], subgroup);
                subgroup.SetParent((T)this);
            }
            var newParts = nameParts.Skip(1).ToArray();
            Subgroups[nameParts[0]].AddSubgroup(newParts, subgroup, disallowDuplicates);
        }
    }
}
