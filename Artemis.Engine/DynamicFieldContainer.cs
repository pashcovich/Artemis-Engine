#region Using Statements

using System;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace Artemis.Engine
{

    /// <summary>
    /// A class that contains fields of arbitrary types which are accessed dynamically
    /// via a string name.
    /// </summary>
    public class DynamicFieldContainer : IEnumerable<object>
    {

        private interface IDynamicField
        {
            object GetUntyped();
        }

        /// <summary>
        /// A field with a explicit getter and setter function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class DynamicField<T> : IDynamicField
        {
            public Func<T> Getter;
            public Action<T> Setter;

            public DynamicField(Func<T> getter)
                : this(getter, null) { }

            public DynamicField(Action<T> setter)
                : this(null, setter) { }

            public DynamicField(Func<T> getter, Action<T> setter)
            {
                Getter = getter;
                Setter = setter;
            }

            public object GetUntyped() { return Getter(); }
        }

        /// <summary>
        /// List of fields of and ArtemisObject
        /// </summary>
        internal Dictionary<string, Object> Fields { get; private set; }

        /// <summary>
        /// Dictionary of dynamic fields (which have custom getters and setters).
        /// </summary>
        private Dictionary<string, IDynamicField> DynamicFields { get; set; }

        public DynamicFieldContainer()
        {
            Fields = new Dictionary<string, Object>();
            DynamicFields = new Dictionary<string, IDynamicField>();
        }

        /// <summary>
        /// Get value of field
        /// </summary>
        public T Get<T>(string name)
        {
            if (Fields.ContainsKey(name))
            {
                return (T)Fields[name];
            }
            else if (DynamicFields.ContainsKey(name))
            {
                var getter = ((DynamicField<T>)DynamicFields[name]).Getter;
                if (getter == null)
                {
                    throw new DynamicFieldException(
                        String.Format(
                            "No getter was found for the field with name '{0}'.", name
                            )
                        );
                }
                return getter();
            }
            throw new DynamicFieldException(
                String.Format(
                    "No field was found with the name '{0}'.", name
                    )
                );
        }

        /// <summary>
        /// Set the value of an field. If the field does not exist, it will be created.
        /// </summary>
        public void Set<T>(string name, T obj)
        {
            if (Fields.ContainsKey(name))
            {
                Fields[name] = obj;
            }
            else if (DynamicFields.ContainsKey(name))
            {
                var setter = ((DynamicField<T>)DynamicFields[name]).Setter;
                if (setter == null)
                {
                    throw new DynamicFieldException(
                        String.Format(
                            "No setter was found for the field with name '{0}'.", name
                            )
                        );
                }
                setter(obj);
            }
            else
            {
                Fields.Add(name, obj);
            }
        }

        /// <summary>
        /// Set the getter for a field. If the field does not exist, it will be created.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="getter"></param>
        public void Set<T>(string name, Func<T> getter)
        {
            Set<T>(name, getter, null);
        }

        /// <summary>
        /// Set the setter for a field. If the field does not exist, it will be created.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="setter"></param>
        public void Set<T>(string name, Action<T> setter)
        {
            Set<T>(name, null, setter);
        }

        /// <summary>
        /// Set the getter and setter for a field. If the field does not exist, it will be created.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        public void Set<T>(string name, Func<T> getter, Action<T> setter)
        {
            if (DynamicFields.ContainsKey(name))
            {
                var field = (DynamicField<T>)DynamicFields[name];
                field.Getter = getter;
                field.Setter = setter;
            }
            else
            {
                DynamicFields.Add(name, new DynamicField<T>(getter, setter));
            }
        }

        /// <summary>
        /// Determine whether or not a field with the given name exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasField(string name)
        {
            return Fields.ContainsKey(name) || DynamicFields.ContainsKey(name);
        }

        /// <summary>
        /// Iterate through every field name.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<object> IterateNames()
        {
            foreach (var kvp in Fields)
            {
                yield return kvp.Key;
            }
            foreach (var kvp in DynamicFields)
            {
                yield return kvp.Key;
            }
        }

        public IEnumerator<object> GetEnumerator()
        {
            foreach (var kvp in Fields)
            {
                yield return kvp.Value;
            }
            foreach (var kvp in DynamicFields)
            {
                var getter = (IDynamicField)kvp.Value;
                yield return getter.GetUntyped();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }
    }
}
