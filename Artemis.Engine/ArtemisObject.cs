using System;
using System.Linq;
using System.Reflection;
using Artemis.Engine.Utilities;

namespace Artemis.Engine
{
    public class ArtemisObject
    {
        /// <summary>
        /// The list of attributes attached to the object
        /// </summary>
        public readonly DynamicFieldContainer Fields;

        /// <summary>
        /// Decides whether or not to update object
        /// </summary>
        public bool NeedsUpdate { get; internal set; }

        private Action updater;

        protected ArtemisObject()
        {

            Fields = new DynamicFieldContainer();

            var thisType = this.GetType();

            // If this object is marked with a HasDynamicPropertiesAttribute then
            // we have to add all the properties marked with DynamicPropertyAttribute
            // to the DynamicFieldContainer.
            if (Reflection.HasAttribute<HasDynamicPropertiesAttribute>(thisType))
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

            Fields.Set<T>(name, getter, setter);
        }

        /// <summary>
        /// Sets the ArtemisObject's current updater to 'updater'
        /// </summary>
        public void SetUpdater(Action updater)
        {
            this.updater = updater;
        }

        /// <summary>
        /// Updates the ArtemisObject by calling its updater
        /// </summary>
        public void Update()
        {
            if (updater != null)
            {
                updater();
            }

            NeedsUpdate = false;
        }
    }
}