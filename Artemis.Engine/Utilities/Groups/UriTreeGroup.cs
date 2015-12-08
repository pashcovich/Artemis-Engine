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
    public class UriTreeGroup<T, U> : UriTreeNode<T> where T : UriTreeGroup<T, U>
    {
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
        /// The dictionary of all items in this group.
        /// </summary>
        public Dictionary<string, U> Items { get; protected set; }

        protected UriTreeGroup(string name)
            : base(name)
        {
            Items = new Dictionary<string, U>();
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
    }
}
