#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#endregion

namespace Artemis.Engine.Utilities.UriTree
{ 
    /// <summary>
    /// An externally mutable version of UriTreeGroup that allows users to
    /// add, set, and insert items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public class UriTreeMutableGroup<T, U> : UriTreeGroup<T, U> where T : UriTreeMutableGroup<T, U>
    {

        public delegate void UriTreeItemDelegate(string itemName, U item);

        public UriTreeItemDelegate OnItemAdded;
        public UriTreeItemDelegate OnItemRemoved;

        protected UriTreeMutableGroup(string name) : base(name) { }

        /// <summary>
        /// Add an item with the given full name to this group.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <param name="disallowDuplicates"></param>
        public void AddItem(string name, U item, bool disallowDuplicates = true)
        {
            AddItem(new Queue<string>(UriUtilities.GetParts(name)), item, disallowDuplicates);
        }

        /// <summary>
        /// Set a pre-existing item with the given full name to the given value.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        public void SetItem(string name, U item)
        {
            AddItem(new Queue<string>(UriUtilities.GetParts(name)), item, false);
        }

        private void AddItem(Queue<string> nameParts, U item, bool disallowDuplicates)
        {
            var first = nameParts.Dequeue();
            if (nameParts.Count == 0)
            {
                if (Items.ContainsKey(first))
                {
                    if (disallowDuplicates)
                    {
                        throw new UriTreeException(
                            String.Format(
                                "Could not add item '{0}' with name '{1}' to group with full name '{2}', " +
                                "an item with that name already exists.", item, first, FullName
                                )
                            );
                    }
                    Items[first] = item;
                }
                else
                {
                    Items.Add(first, item);
                }
                if (OnItemAdded != null)
                    OnItemAdded(first, item);
            }
            else
            {
                if (!Subnodes.ContainsKey(first))
                {
                    throw new UriTreeException(
                        String.Format(
                            "Could not add item '{0}' with name '{1}' to group with full name '{2}; " +
                            "missing intermediate node with name '{3}'. Consider using InsertItem instead.",
                            item, nameParts, FullName, first
                            )
                        );
                }
                Subnodes[first].AddItem(nameParts, item, disallowDuplicates);
            }
        }

        /// <summary>
        /// Add an anonymous item to this group.
        /// </summary>
        /// <param name="item"></param>
        public void AddAnonymousItem(U item)
        {
            AnonymousItems.Add(item);
        }

        /// <summary>
        /// Add an anonymous item to the subgroup of this group with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public void AddAnonymousItem(string name, U item)
        {
            AddAnonymousItem(new Queue<string>(UriUtilities.GetParts(name)), item);
        }

        private void AddAnonymousItem(Queue<string> nameParts, U item)
        {
            if (nameParts.Count == 0)
            {
                AnonymousItems.Add(item);
                if (OnItemAdded != null)
                    OnItemAdded(null, item);
            }
            else
            {
                var first = nameParts.Dequeue();
                if (!Subnodes.ContainsKey(first))
                {
                    throw new UriTreeException(
                        String.Format(
                            "Could not add item '{0}' to group with full name '{1}'; " +
                            "missing intermediate node with name '{2}'. Consider using InsertItem instead.",
                            item, FullName, first
                            )
                        );
                }
                Subnodes[first].AddAnonymousItem(nameParts, item);
            }
        }

        /// <summary>
        /// Insert an item into the UriTree with the given full name. Inserting an item is
        /// different from adding an item in the sense that missing intermediate nodes will
        /// be automatically created (i.e. adding "a.b.c.d" to "a.b" will automatically create
        /// the missing node "a.b.c").
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <param name="disallowDuplicates"></param>
        public void InsertItem(string name, U item, bool disallowDuplicates = true)
        {
            InsertItem<T>(new Queue<string>(UriUtilities.GetParts(name)), item, disallowDuplicates);
        }

        /// <summary>
        /// Insert an item into the UriTree with the given full name. Inserting an item is
        /// different from adding an item in the sense that missing intermediate nodes will
        /// be automatically created (i.e. adding "a.b.c.d" to "a.b" will automatically create
        /// the missing node "a.b.c").
        /// 
        /// The NodeType generic parameter is the type of instance used to create missing
        /// intermediate nodes. For this method to work, the supplied NodeType must have a
        /// constructor that takes a single string argument and calls base(string).
        /// </summary>
        /// <typeparam name="NodeType"></typeparam>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <param name="disallowDuplicates"></param>
        public void InsertItem<NodeType>(string name, U item, bool disallowDuplicates = true)
            where NodeType : UriTreeMutableGroup<T, U>
        {
            InsertItem<NodeType>(new Queue<string>(UriUtilities.GetParts(name)), item, disallowDuplicates);
        }

        private void InsertItem<NodeType>(Queue<string> nameParts, U item, bool disallowDuplicates)
            where NodeType : UriTreeMutableGroup<T, U>
        {
            var first = nameParts.Dequeue();

            if (nameParts.Count == 1)
            {
                if (Items.ContainsKey(first))
                {
                    if (disallowDuplicates)
                    {
                        throw new UriTreeException(
                            String.Format(
                                "Cannot insert item '{0}' with name '{1}' to group with full name '{2}', " +
                                "an item with that name already exists.", item, first, FullName
                                )
                            );
                    }
                    Items[first] = item;
                }
                else
                {
                    Items.Add(first, item);
                }
                if (OnItemAdded != null)
                    OnItemAdded(first, item);
            }
            else
            {
                if (!Subnodes.ContainsKey(first))
                {
                    var newNode = (T)Activator.CreateInstance(typeof(NodeType), first);
                    newNode.SetParent((T)this); // will add to Subnodes automatically.
                }
                Subnodes[first].InsertItem<NodeType>(nameParts, item, disallowDuplicates);
            }
        }

        /// <summary>
        /// Insert an anonymous item into the subgroup of this group with the
        /// given name. Unlike AddAnonymousItem, this will automatically create
        /// any missing intermediate groups (i.e. adding "a.b.c.d.e" to "a.b.c"
        /// will create "a.b.c.d").
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public void InsertAnonymousItem(string name, U item)
        {
            InsertAnonymousItem<T>(name, item);
        }

        /// <summary>
        /// Insert an anonymous item into the subgroup of this group with the
        /// given name. Unlike AddAnonymousItem, this will automatically create
        /// any missing intermediate groups (i.e. adding "a.b.c.d.e" to "a.b.c"
        /// will create "a.b.c.d").
        /// 
        /// The type of group used is the NodeType generic parameter. For this
        /// method to work, the given NodeType must have a constructor that takes
        /// a single string argument and calls base(string).
        /// </summary>
        /// <typeparam name="NodeType"></typeparam>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public void InsertAnonymousItem<NodeType>(string name, U item)
            where NodeType : UriTreeMutableGroup<T, U>
        {
            InsertAnonymousItem<NodeType>(new Queue<string>(UriUtilities.GetParts(name)), item);
        }

        private void InsertAnonymousItem<NodeType>(Queue<string> nameParts, U item)
            where NodeType : UriTreeMutableGroup<T, U>
        {
            if (nameParts.Count == 0)
            {
                AnonymousItems.Add(item);
                if (OnItemAdded != null)
                    OnItemAdded(null, item);
            }
            else
            {
                var first = nameParts.Dequeue();
                if (!Subnodes.ContainsKey(first))
                {
                    var newNode = (T)Activator.CreateInstance(typeof(NodeType), first);
                    newNode.SetParent((T)this);
                }
                Subnodes[first].InsertAnonymousItem<NodeType>(nameParts, item);
            }
        }
        
        /// <summary>
        /// Remove the item with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="failQuiet"></param>
        public void RemoveItem(string name, bool failQuiet = false)
        {
            RemoveItem(new Queue<string>(UriUtilities.GetParts(name)), failQuiet);
        }

        private void RemoveItem(Queue<string> nameParts, bool failQuiet)
        {
            var first = nameParts.Dequeue();
            if (nameParts.Count == 0)
            {
                if (!Items.ContainsKey(first))
                {
                    if (failQuiet)
                    {
                        return;
                    }
                    throw CouldNotRemoveItem(nameParts);
                }
                var item = Items[first];
                Items.Remove(first);
                if (OnItemRemoved != null)
                    OnItemRemoved(first, item);
            }
            else
            {
                if (!Subnodes.ContainsKey(first))
                {
                    if (failQuiet)
                    {
                        return;
                    }
                    throw CouldNotRemoveItem(nameParts);
                }
                Subnodes[first].RemoveItem(nameParts, failQuiet);
            }
        }

        private UriTreeException CouldNotRemoveItem(Queue<string> nameParts)
        {
            var name = String.Join(UriUtilities.URI_SEPARATOR.ToString(), nameParts);
            return new UriTreeException(
                String.Format(
                    "Could not remove item with full name '{0}' from group with full name " +
                    "'{1}'; item does not exist.",
                    name, FullName
                    )
                );
        }

        /// <summary>
        /// Remove an anonymous item from this group.
        /// </summary>
        /// <param name="item"></param>
        public bool RemoveAnonymousItem(U item, bool searchRecursive = true)
        {
            if (!AnonymousItems.Remove(item) && searchRecursive)
            {
                if (IsLeaf)
                    return false;
                return Subnodes.Any(kvp => kvp.Value.RemoveAnonymousItem(item, searchRecursive));
            }
            if (OnItemRemoved != null)
                OnItemRemoved(null, item);
            return true;
        }

        /// <summary>
        /// Remove an anonymous item from the subgroup of this group with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        /// <param name="failQuiet"></param>
        public bool RemoveAnonymousItem(string name, U item, bool failQuiet = true)
        {
            return RemoveAnonymousItem(new Queue<string>(UriUtilities.GetParts(name)), item, failQuiet);
        }

        private bool RemoveAnonymousItem(Queue<string> nameParts, U item, bool failQuiet)
        {
            if (nameParts.Count == 0)
            {
                var removed = AnonymousItems.Remove(item);
                if (OnItemRemoved != null)
                    OnItemRemoved(null, item);
                return removed;
            }
            else
            {
                var first = nameParts.Dequeue();
                if (!Subnodes.ContainsKey(first))
                {
                    if (failQuiet)
                    {
                        return false;
                    }
                    throw new UriTreeException(
                        String.Format(
                            "Could not remove item '{0}' from group with full name '{1}'; " +
                            "missing intermediate node with name '{2}'.",
                            item, FullName, first
                            )
                        );
                }
                return Subnodes[first].RemoveAnonymousItem(nameParts, item, failQuiet);
            }
        }

        /// <summary>
        /// Clear all the items (named or anonymous).
        /// </summary>
        /// <param name="recursive"></param>
        public void ClearItems(bool recursive = false)
        {
            if (OnItemRemoved != null)
            {
                foreach (var kvp in Items)
                    OnItemRemoved(kvp.Key, kvp.Value);
                foreach (var item in AnonymousItems)
                    OnItemRemoved(null, item);

                Items.Clear();
                AnonymousItems.Clear();
            }
            else
            {
                Items.Clear();
                AnonymousItems.Clear();
            }

            if (recursive)
            {
                foreach (var subnode in Subnodes.Values)
                    subnode.ClearItems(recursive);
            }
        }

        /// <summary>
        /// Clear all the named items.
        /// </summary>
        /// <param name="recursive"></param>
        public void ClearNamedItems(bool recursive = false)
        {
            if (OnItemRemoved != null)
            {
                foreach (var kvp in Items)
                    OnItemRemoved(kvp.Key, kvp.Value);
                Items.Clear();
            }
            else
            {
                Items.Clear();
            }

            if (recursive)
            {
                foreach (var subnode in Subnodes.Values)
                    subnode.ClearNamedItems(recursive);
            }
        }

        /// <summary>
        /// Clear all named items whose name matches the given regex.
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="recursive"></param>
        public void ClearNamedItems(string regex, bool recursive = false)
        {
            var toRemove = new List<string>();
            if (OnItemRemoved != null)
            {
                foreach (var kvp in Items)
                {
                    if (Regex.IsMatch(kvp.Key, regex))
                    {
                        toRemove.Add(kvp.Key);
                        OnItemRemoved(kvp.Key, kvp.Value);
                    }
                }
            }
            else
            {
                foreach (var kvp in Items)
                {
                    if (Regex.IsMatch(kvp.Key, regex))
                        toRemove.Add(kvp.Key);
                }
            }
            foreach (var name in toRemove)
                Items.Remove(name);

            if (recursive)
            {
                foreach (var subnode in Subnodes.Values)
                    subnode.ClearNamedItems(regex, recursive);
            }
        }

        /// <summary>
        /// Clear all anonymous items.
        /// </summary>
        /// <param name="recursive"></param>
        public void ClearAnonymousItems(bool recursive = false)
        {
            if (OnItemRemoved != null)
            {
                foreach (var item in AnonymousItems)
                    OnItemRemoved(null, item);
                AnonymousItems.Clear();
            }
            else
            {
                AnonymousItems.Clear();
            }

            if (recursive)
            {
                foreach (var subnode in Subnodes.Values)
                {
                    subnode.ClearAnonymousItems(recursive);
                }
            }
        }
    }
}
