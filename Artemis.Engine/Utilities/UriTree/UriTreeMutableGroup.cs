#region Using Statements

using System;
using System.Linq;

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
        protected UriTreeMutableGroup(string name) : base(name) { }

        /// <summary>
        /// Add an item with the given full name to this group.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <param name="disallowDuplicates"></param>
        public void AddItem(string name, U item, bool disallowDuplicates = true)
        {
            AddItem(UriUtilities.GetParts(name), item, disallowDuplicates);
        }

        /// <summary>
        /// Set a pre-existing item with the given full name to the given value.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        public void SetItem(string name, U item)
        {
            AddItem(UriUtilities.GetParts(name), item, false);
        }

        private void AddItem(string[] nameParts, U item, bool disallowDuplicates)
        {
            if (nameParts.Length == 1)
            {
                var name = nameParts[0];
                if (Items.ContainsKey(name))
                {
                    if (disallowDuplicates)
                    {
                        throw new UriTreeException(
                            String.Format(
                                "Could not add item '{0}' with name '{1}' to group with full name '{2}', " +
                                "an item with that name already exists.", item, name, FullName
                                )
                            );
                    }
                    Items[name] = item;
                }
                else
                {
                    Items.Add(name, item);
                }
            }
            else
            {
                var newParts = nameParts.Skip(1).ToArray();
                if (!Subnodes.ContainsKey(nameParts[0]))
                {
                    throw new UriTreeException(
                        String.Format(
                            "Could not add item '{0}' with name '{1}' to group with full name '{2}; " +
                            "missing intermediate node with name '{3}'. Consider using InsertItem instead.",
                            item, nameParts, FullName, nameParts[0]
                            )
                        );
                }
                Subnodes[nameParts[0]].AddItem(newParts, item, disallowDuplicates);
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
            AddAnonymousItem(UriUtilities.GetParts(name), item);
        }

        private void AddAnonymousItem(string[] nameParts, U item)
        {
            if (nameParts.Length == 0)
            {
                AnonymousItems.Add(item);
            }
            else
            {
                if (!Subnodes.ContainsKey(nameParts[0]))
                {
                    throw new UriTreeException(
                        String.Format(
                            "Could not add item '{0}' to group with full name '{1}'; " +
                            "missing intermediate node with name '{2}'. Consider using InsertItem instead.",
                            item, FullName, nameParts[0]
                            )
                        );
                }
                var newParts = nameParts.Skip(1).ToArray();
                Subnodes[nameParts[0]].AddAnonymousItem(newParts, item);
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
            InsertItem<T>(UriUtilities.GetParts(name), item, disallowDuplicates);
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
            InsertItem<NodeType>(UriUtilities.GetParts(name), item, disallowDuplicates);
        }

        private void InsertItem<NodeType>(string[] nameParts, U item, bool disallowDuplicates)
            where NodeType : UriTreeMutableGroup<T, U>
        {
            var firstPart = nameParts[0];

            if (nameParts.Length == 1)
            {
                if (Items.ContainsKey(firstPart))
                {
                    if (disallowDuplicates)
                    {
                        throw new UriTreeException(
                            String.Format(
                                "Cannot insert item '{0}' with name '{1}' to group with full name '{2}', " +
                                "an item with that name already exists.", item, firstPart, FullName
                                )
                            );
                    }
                    Items[firstPart] = item;
                }
                else
                {
                    Items.Add(firstPart, item);
                }
            }
            else
            {
                if (!Subnodes.ContainsKey(firstPart))
                {
                    var newNode = (T)Activator.CreateInstance(typeof(NodeType), firstPart);
                    newNode.SetParent((T)this); // will add to Subnodes automatically.
                }
                var newParts = nameParts.Skip(1).ToArray();
                Subnodes[firstPart].InsertItem<NodeType>(nameParts, item, disallowDuplicates);
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
            InsertAnonymousItem<NodeType>(UriUtilities.GetParts(name), item);
        }

        private void InsertAnonymousItem<NodeType>(string[] nameParts, U item)
            where NodeType : UriTreeMutableGroup<T, U>
        {
            if (nameParts.Length == 0)
            {
                AnonymousItems.Add(item);
            }
            else
            {
                var firstPart = nameParts[0];
                if (!Subnodes.ContainsKey(firstPart))
                {
                    var newNode = (T)Activator.CreateInstance(typeof(NodeType), firstPart);
                    newNode.SetParent((T)this);
                }
                var newParts = nameParts.Skip(1).ToArray();
                Subnodes[firstPart].InsertAnonymousItem<NodeType>(newParts, item);
            }
        }
        
        /// <summary>
        /// Remove the item with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="failQuiet"></param>
        public void RemoveItem(string name, bool failQuiet = false)
        {
            RemoveItem(UriUtilities.GetParts(name), failQuiet);
        }

        private void RemoveItem(string[] nameParts, bool failQuiet)
        {
            if (nameParts.Length == 1)
            {
                var name = nameParts[0];
                if (!Items.ContainsKey(name))
                {
                    if (failQuiet)
                    {
                        return;
                    }
                    throw CouldNotRemoveItem(nameParts);
                }
                Items.Remove(name);
            }
            else
            {
                if (!Subnodes.ContainsKey(nameParts[0]))
                {
                    if (failQuiet)
                    {
                        return;
                    }
                    throw CouldNotRemoveItem(nameParts);
                }
                var newParts = nameParts.Skip(1).ToArray();
                Subnodes[nameParts[0]].RemoveItem(newParts, failQuiet);
            }
        }

        private UriTreeException CouldNotRemoveItem(string[] nameParts)
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
        public void RemoveAnonymousItem(U item)
        {
            AnonymousItems.Remove(item);
        }

        /// <summary>
        /// Remove an anonymous item from the subgroup of this group with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        /// <param name="failQuiet"></param>
        public void RemoveAnonymousItem(string name, U item, bool failQuiet = true)
        {
            RemoveAnonymousItem(UriUtilities.GetParts(name), item, failQuiet);
        }

        private void RemoveAnonymousItem(string[] nameParts, U item, bool failQuiet)
        {
            if (nameParts.Length == 0)
            {
                AnonymousItems.Remove(item);
            }
            else
            {
                if (!Subnodes.ContainsKey(nameParts[0]))
                {
                    if (failQuiet)
                    {
                        return;
                    }
                    throw new UriTreeException(
                        String.Format(
                            "Could not remove item '{0}' from group with full name '{1}'; " +
                            "missing intermediate node with name '{2}'.",
                            item, FullName, nameParts[0]
                            )
                        );
                }
                var newParts = nameParts.Skip(1).ToArray();
                Subnodes[nameParts[0]].RemoveAnonymousItem(newParts, item, failQuiet);
            }
        }
    }
}
