#region Using Statements

using Artemis.Engine.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace Artemis.Engine
{

    /// <summary>
    /// An ArtemisObject is the most generic form of "game object" found in the ArtemisEngine.
    /// </summary>
    public class ArtemisObject : TimeableObject
    {

        // The following dictionary maps from an Attribute Type object to a dictionary of known
        // subclasses of ArtemisObject. The nested dictionary maps from the sub Type of ArtemisObject
        // to a bool determining whether or not the subtype has said attribute applied to it.
        //
        // This speeds up the lookup in ArtemisObject's constructor so that creating a bunch of
        // instances of a subclass of ArtemisObject doesn't result in tons of calls to HasAttribute,
        // which is worlds slower than a Dictionary lookup.

        private static Dictionary<Type, Dictionary<Type, bool>> knownSubclassAttributes
            = new Dictionary<Type, Dictionary<Type, bool>>();

        static ArtemisObject()
        {
            knownSubclassAttributes.Add(typeof(HasDynamicPropertiesAttribute), new Dictionary<Type, bool>());
            knownSubclassAttributes.Add(typeof(ForceUpdateAttribute), new Dictionary<Type, bool>());
        }

        /// <summary>
        /// Checks if the given type is contained in the given dictionary of known
        /// types with the given attribute, and if it is performs the given action,
        /// otherwise checks if it possesses the given attribute. If it does, it's
        /// added to the dictionary with value "true" and the action is performed,
        /// otherwise it's added to the dictionary with value "false".
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attr"></param>
        /// <param name="hasAttrDict"></param>
        /// <param name="action"></param>
        private static void CheckForAttributeAndPerformAction(Type type, Type attr, Action action)
        {
            var knownSubclasses = knownSubclassAttributes[attr];
            if (!knownSubclasses.ContainsKey(type))
            {
                if (Reflection.HasAttribute(type, attr))
                {
                    action();
                    knownSubclasses.Add(type, true);
                }
                else
                {
                    knownSubclasses.Add(type, false);
                }
            }
            else if (knownSubclasses[type])
            {
                action();
            }
        }

        /// <summary>
        /// The list of attributes attached to the object
        /// </summary>
        public readonly DynamicFieldContainer Fields;

        /// <summary>
        /// Decides whether or not to update object
        /// </summary>
        public bool NeedsUpdate { get; internal set; }

        private Action updater;
        private bool managedByGlobalUpdater;

        protected ArtemisObject()
            : base()
        {
            Fields = new DynamicFieldContainer();

            var thisType = this.GetType();

            // If this object is marked with a HasDynamicPropertiesAttribute then
            // we have to add all the properties marked with DynamicPropertyAttribute
            // to the DynamicFieldContainer.
            CheckForAttributeAndPerformAction(
                thisType, typeof(HasDynamicPropertiesAttribute),  
                () => SetupDynamicProperties(thisType));

            CheckForAttributeAndPerformAction(
                thisType, typeof(ForceUpdateAttribute),
                () =>
                {
                    ArtemisEngine.GameUpdater.Add(this);
                    managedByGlobalUpdater = true;
                });
        }

        /// <summary>
        /// Setup the object if it has a HasDynamicProperties attribute.
        /// </summary>
        /// <param name="thisType"></param>
        private void SetupDynamicProperties(Type thisType)
        {
            var attribute = (HasDynamicPropertiesAttribute)Attribute.GetCustomAttribute(
                    thisType, typeof(HasDynamicPropertiesAttribute));

            if (attribute.HasDynamicPropertyList)
            {
                var properties = from name in attribute.DynamicPropertyNames
                                 select thisType.GetProperty(name);
                AddDynamicProperties(properties.ToArray());
            }
            else
            {
                AddDynamicProperties(thisType.GetProperties());
            }
        }

        /// <summary>
        /// Add all the dynamic properties attached to this object to the
        /// DynamicFieldContainer.
        /// </summary>
        /// <param name="properties"></param>
        private void AddDynamicProperties(PropertyInfo[] properties)
        {
            var invocationBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            foreach (var property in properties)
            {
                if (Reflection.HasAttribute<DynamicPropertyAttribute>(property))
                {
                    var type = property.PropertyType;

                    Reflection.InvokeGenericMethod(
                        this, "AddDynamicProperty", type, 
                        invocationBindingFlags, property);
                }
                else
                {
                    throw new DynamicPropertyException(
                        String.Format(
                            "The property with name '{0}' is not marked " +
                            "with a DynamicPropertyAttribute.", property.Name
                            )
                        );
                }
            }
        }

        /// <summary>
        /// Add a single DynamicProperty to the DynamicFieldContainer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyInfo"></param>
        protected void AddDynamicProperty<T>(PropertyInfo propertyInfo)
        {
            var name = propertyInfo.Name;

            var propGetter = propertyInfo.GetGetMethod();
            var propSetter = propertyInfo.GetSetMethod();

            Func<T> getter = () => (T)propGetter.Invoke(this, null);
            Action<T> setter = (obj) => propSetter.Invoke(this, new object[] { obj });

            Fields.SetGetterAndSetter<T>(name, getter, setter);
        }

        /// <summary>
        /// Sets the ArtemisObject's current updater to 'updater'
        /// </summary>
        public void SetUpdater(Action updater)
        {
            this.updater = updater;
        }

        // NOTE: It may seem counterintuitive that the Update method is internal
        // virtual instead of protected virtual or public virtual. This is because
        // the ACTUAL way the user updates ArtemisObjects is by supplying an Updater
        // function with SetUpdater. So they really have no reason to override this
        // themselves and mess up things they don't know about.

        /// <summary>
        /// Updates the ArtemisObject by calling its updater
        /// </summary>
        internal virtual void Update()
        {
            UpdateTime();

            if (updater != null)
            {
                updater();
            }

            NeedsUpdate = false;
        }

        protected override void Kill()
        {
            if (managedByGlobalUpdater)
            {
                ArtemisEngine.GameUpdater.Remove(this);
            }
        }
    }
}