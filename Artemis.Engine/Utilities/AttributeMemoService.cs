#region Using Statements

using System;
using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Utilities
{
    public class AttributeMemoService<T> where T : class
    {

        /// <summary>
        /// Handler called when an instance has a certain attribute.
        /// </summary>
        /// <param name="instance"></param>
        public delegate void AttributeHandler(T instance);

        private List<Type> registeredAttributes;

        // Maps a type to a set of the attribute types that are applied to it.
        private Dictionary<Type, List<AttributeHandler>> memoizedSubclasses;

        private Dictionary<Type, AttributeHandler> attributePresentHandlers;
        private Dictionary<Type, AttributeHandler> attributeMissingHandlers;

        public AttributeMemoService() 
        {
            registeredAttributes = new List<Type>();
            memoizedSubclasses = new Dictionary<Type, List<AttributeHandler>>();
            attributePresentHandlers = new Dictionary<Type, AttributeHandler>();
            attributeMissingHandlers = new Dictionary<Type, AttributeHandler>();
        }

        /// <summary>
        /// Register handlers that are invoked in "Handle" when an instance has, or is missing,
        /// the given attribute type.
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="presentHandler"></param>
        /// <param name="missingHandler"></param>
        public void RegisterHandler(
            Type attributeType, AttributeHandler presentHandler, AttributeHandler missingHandler)
        {
            if (presentHandler != null)
            {
                attributePresentHandlers.Add(attributeType, presentHandler);
            }
            if (missingHandler != null)
            {
                attributeMissingHandlers.Add(attributeType, missingHandler);
            }
        }

        /// <summary>
        /// Register handlers that are invoked in "Handle" when an instance has, or is missing,
        /// the given attribute type.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="presentHandler"></param>
        /// <param name="missingHandler"></param>
        public void RegisterHandler<U>(AttributeHandler presentHandler, AttributeHandler missingHandler)
            where U : Attribute
        {
            RegisterHandler(typeof(U), presentHandler, missingHandler);
        }

        /// <summary>
        /// Register handlers that are invoked in "Handle" when an instance has the given attribute
        /// type.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="presentHandler"></param>
        public void RegisterHandler<U>(AttributeHandler presentHandler)
            where U : Attribute
        {
            RegisterHandler(typeof(U), presentHandler, null);
        }

        /// <summary>
        /// Given an object, determine the attributes associated with its type
        /// and perform the according actions.
        /// </summary>
        /// <param name="instance"></param>
        public void Handle(T instance)
        {
            var type = instance.GetType();

            if (!memoizedSubclasses.ContainsKey(type))
            {
                var handlers = new List<AttributeHandler>();

                foreach (var attribute in registeredAttributes)
                {
                    if (Reflection.HasAttribute(type, attribute))
                    {
                        if (attributePresentHandlers.ContainsKey(type))
                        {
                            handlers.Add(attributePresentHandlers[type]);
                        }
                    }
                    else
                    {
                        if (attributeMissingHandlers.ContainsKey(type))
                        {
                            handlers.Add(attributeMissingHandlers[type]);
                        }
                    }
                }
                memoizedSubclasses.Add(type, handlers);
            }

            foreach (var handler in memoizedSubclasses[type])
            {
                handler(instance);
            }
        }

    }
}
