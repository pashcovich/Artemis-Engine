#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#endregion

namespace Artemis.Engine.Utilities.UriTree
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

        /// <summary>
        /// The list of all unnamed items in this group.
        /// </summary>
        public ICollection<U> AnonymousItems { get; protected set; }

        protected UriTreeGroup(string name, ICollection<U> anonymousItemsCollection = null)
            : base(name)
        {
            Items = new Dictionary<string, U>();
            AnonymousItems = anonymousItemsCollection == null ? anonymousItemsCollection : new HashSet<U>();
        }

        /// <summary>
        /// Return the item with the given full name. If not found, return the given
        /// default value.
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public U GetItem(string fullName, U defaultVal, bool useDefault = true)
        {
            return GetItem(new Queue<string>(UriUtilities.GetParts(fullName)), useDefault, defaultVal);
        }

        /// <summary>
        /// Return the item with the given full name.
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public U GetItem(string fullName, bool useDefault = false)
        {
            return GetItem(new Queue<string>(UriUtilities.GetParts(fullName)), useDefault, default(U));
        }

        internal U GetItem(Queue<string> nameParts, bool useDefault, U defaultVal)
        {
            var first = nameParts.Dequeue();
            if (nameParts.Count > 0 && !Subnodes.ContainsKey(first))
            {
                if (useDefault)
                {
                    return defaultVal;
                }
                throw CouldNotRetrieveItem(first);
            }
            else if (nameParts.Count == 0)
            {
                if (!Items.ContainsKey(first))
                {
                    if (useDefault)
                    {
                        return defaultVal;
                    }
                    throw CouldNotRetrieveItem(first);
                }
                return Items[first];
            }
            return Subnodes[first].GetItem(nameParts, useDefault, defaultVal);
        }

        /// <summary>
        /// Common exception thrown when an item could not be retrieved.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private UriTreeException CouldNotRetrieveItem(string name)
        {
            return new UriTreeException(
                    String.Format(
                        "Could not retrieve item with name '{0}' " +
                        "from group with full name '{1}'", name, FullName
                        )
                    );
        }

        public bool ContainsItem(string name)
        {
            return ContainsItem(new Queue<string>(UriUtilities.GetParts(name)));
        }

        internal bool ContainsItem(Queue<string> nameParts)
        {
            var first = nameParts.Dequeue();
            if (nameParts.Count == 0)
            {
                return Items.ContainsKey(first);
            }
            return Subnodes[first].ContainsItem(nameParts);
        }

        /// <summary>
        /// Check if this group contains the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool ContainsItem(U item, bool searchRecursive = true)
        {
            return Items.ContainsValue(item) || (
                        searchRecursive ? Subnodes.Any(kvp => kvp.Value.ContainsItem(item)) : false
                        );
        }

        /// <summary>
        /// Iterate through all the items in this group.
        /// 
        /// Note: This will not iterate through all items in subgroups as well.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<U> IterateItems()
        {
            foreach (var kvp in Items)
            {
                yield return kvp.Value;
            }

            foreach (var item in AnonymousItems)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Iterate through all the named items in this group.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<U> IterateNamedItems()
        {
            foreach (var kvp in Items)
            {
                yield return kvp.Value;
            }
        }

        /// <summary>
        /// Iterate through all the anonymous items in this group.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<U> IterateAnonymousItems()
        {
            foreach (var item in AnonymousItems)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Iterate through all the items in this group whose name matches a
        /// given regex.
        /// 
        /// Note: This will not iterate through all items in subgroups as well.
        /// </summary>
        /// <param name="regex"></param>
        /// <returns></returns>
        public IEnumerator<U> IterateItems(string regex)
        {
            foreach (var kvp in Items)
            {
                if (Regex.IsMatch(kvp.Key, regex))
                {
                    yield return kvp.Value;
                }
            }
        }

        /// <summary>
        /// Iterate through the names of items.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> IterateItemNames()
        {
            foreach (var kvp in Items)
            {
                yield return kvp.Key;
            }
        }
    }
}
