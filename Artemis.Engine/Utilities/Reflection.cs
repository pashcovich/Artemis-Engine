#region Using Statements

using Artemis.Engine.Utilities.UriTree;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace Artemis.Engine.Utilities
{

    /// <summary>
    /// A static class containing convenience methods for certain reflection
    /// related operations, such as retrieving attributes from an object.
    /// </summary>
    internal static class Reflection
    {

        private static List<Attribute> _GetAttributes(Attribute[] all, Type attr)
        {
            var attrs = new List<Attribute>();
            foreach (var other_attr in all)
            {
                if (other_attr.GetType() == attr)
                {
                    attrs.Add(other_attr);
                }
            }
            return attrs;
        }

        private static List<T> _GetAttributes<T>(Attribute[] all)
            where T : Attribute
        {
            var attrs = new List<T>();
            foreach (var other_attr in all)
            {
                var asT = other_attr as T;
                if (asT != null)
                {
                    attrs.Add(asT);
                }
            }
            return attrs;
        }

        #region GetAttributes Overloads

        /// <summary>
        /// Get the attributes of the given type from the given Assembly object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static List<Attribute> GetAttributes(Assembly obj, Type attributeType)
        {
            return _GetAttributes(Attribute.GetCustomAttributes(obj), attributeType);
        }

        /// <summary>
        /// Get the attributes of the given type from the given MemberInfo object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static List<Attribute> GetAttributes(MemberInfo obj, Type attributeType)
        {
            return _GetAttributes(Attribute.GetCustomAttributes(obj), attributeType);
        }

        /// <summary>
        /// Get the attributes of the given type from the given Module object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static List<Attribute> GetAttributes(Module obj, Type attributeType)
        {
            return _GetAttributes(Attribute.GetCustomAttributes(obj), attributeType);
        }

        /// <summary>
        /// Get the attributes of the given type from the given ParameterInfo object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static List<Attribute> GetAttributes(ParameterInfo obj, Type attributeType)
        {
            return _GetAttributes(Attribute.GetCustomAttributes(obj), attributeType);
        }

        /// <summary>
        /// Get the attributes of the same type as the given generic parameter from
        /// the given Assembly object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> GetAttributes<T>(Assembly obj)
            where T : Attribute
        {
            return _GetAttributes<T>(Attribute.GetCustomAttributes(obj));
        }

        /// <summary>
        /// Get the attributes of the same type as the given generic parameter from
        /// the given MemberInfo object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> GetAttributes<T>(MemberInfo obj)
            where T : Attribute
        {
            return _GetAttributes<T>(Attribute.GetCustomAttributes(obj));
        }

        /// <summary>
        /// Get the attributes of the same type as the given generic parameter from
        /// the given Module object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> GetAttributes<T>(Module obj)
            where T : Attribute
        {
            return _GetAttributes<T>(Attribute.GetCustomAttributes(obj));
        }

        /// <summary>
        /// Get the attributes of the same type as the given generic parameter from
        /// the given ParameterInfo object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> GetAttributes<T>(ParameterInfo obj)
            where T : Attribute
        {
            return _GetAttributes<T>(Attribute.GetCustomAttributes(obj));
        }

        #endregion

        #region GetFirstAttribute Overloads

        /// <summary>
        /// Get the first attribute of the given type from the given Assembly object.
        /// This is different from Attribute.GetCustomAttribute because that will throw
        /// an exception if multiple instances of the given attribute are applied to the
        /// object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static Attribute GetFirstAttribute(Assembly obj, Type attributeType)
        {
            return GetAttributes(obj, attributeType).FirstOrDefault();
        }

        /// <summary>
        /// Get the first attribute of the given type from the given MemberInfo object.
        /// This is different from Attribute.GetCustomAttribute because that will throw
        /// an exception if multiple instances of the given attribute are applied to the
        /// object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static Attribute GetFirstAttribute(MemberInfo obj, Type attributeType)
        {
            return GetAttributes(obj, attributeType).FirstOrDefault();
        }

        /// <summary>
        /// Get the first attribute of the given type from the given Module object.
        /// This is different from Attribute.GetCustomAttribute because that will throw
        /// an exception if multiple instances of the given attribute are applied to the
        /// object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static Attribute GetFirstAttribute(Module obj, Type attributeType)
        {
            return GetAttributes(obj, attributeType).FirstOrDefault();
        }

        /// <summary>
        /// Get the first attribute of the given type from the given ParameterInfo object.
        /// This is different from Attribute.GetCustomAttribute because that will throw
        /// an exception if multiple instances of the given attribute are applied to the
        /// object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static Attribute GetFirstAttribute(ParameterInfo obj, Type attributeType)
        {
            return GetAttributes(obj, attributeType).FirstOrDefault();
        }

        /// <summary>
        /// Get the first attribute with the same type as the given generic parameter
        /// from the given Assembly object. This is different from Attribute.GetCustomAttribute 
        /// because that will throw an exception if multiple instances of the given attribute are 
        /// applied to the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetFirstAttribute<T>(Assembly obj)
            where T : Attribute
        {
            return GetAttributes<T>(obj).FirstOrDefault();
        }

        /// <summary>
        /// Get the first attribute with the same type as the given generic parameter
        /// from the given MemberInfo object. This is different from Attribute.GetCustomAttribute 
        /// because that will throw an exception if multiple instances of the given attribute are 
        /// applied to the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetFirstAttribute<T>(MemberInfo obj)
            where T : Attribute
        {
            return GetAttributes<T>(obj).FirstOrDefault();
        }

        /// <summary>
        /// Get the first attribute with the same type as the given generic parameter
        /// from the given Module object. This is different from Attribute.GetCustomAttribute 
        /// because that will throw an exception if multiple instances of the given attribute are 
        /// applied to the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetFirstAttribute<T>(Module obj)
            where T : Attribute
        {
            return GetAttributes<T>(obj).FirstOrDefault();
        }

        /// <summary>
        /// Get the first attribute with the same type as the given generic parameter
        /// from the given ParameterInfo object. This is different from Attribute.GetCustomAttribute 
        /// because that will throw an exception if multiple instances of the given attribute are 
        /// applied to the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetFirstAttribute<T>(ParameterInfo obj)
            where T : Attribute
        {
            return GetAttributes<T>(obj).FirstOrDefault();
        }

        #endregion

        #region HasAttribute Overloads

        /// <summary>
        /// Check if the given Assembly object has an attribute of the given type.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static bool HasAttribute(Assembly obj, Type attributeType)
        {
            return obj.IsDefined(attributeType, false);
        }

        /// <summary>
        /// Check if the given MemberInfo object has an attribute of the given type.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static bool HasAttribute(MemberInfo obj, Type attributeType)
        {
            return obj.IsDefined(attributeType, false);
        }

        /// <summary>
        /// Check if the given Module object has an attribute of the given type.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static bool HasAttribute(Module obj, Type attributeType)
        {
            return obj.IsDefined(attributeType, false);
        }

        /// <summary>
        /// Check if the given ParameterInfo object has an attribute of the given type.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static bool HasAttribute(ParameterInfo obj, Type attributeType)
        {
            return obj.IsDefined(attributeType, false);
        }

        /// <summary>
        /// Check if the given Assembly object has an attribute of the same type as
        /// the given generic parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(Assembly obj)
            where T : Attribute
        {
            return obj.IsDefined(typeof(T), false);
        }

        /// <summary>
        /// Check if the given MemberInfo object has an attribute of the same type as
        /// the given generic parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(MemberInfo obj)
            where T : Attribute
        {
            return obj.IsDefined(typeof(T), false);
        }

        /// <summary>
        /// Check if the given Module object has an attribute of the same type as
        /// the given generic parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(Module obj)
            where T : Attribute
        {
            return obj.IsDefined(typeof(T), false);
        }

        /// <summary>
        /// Check if the given ParameterInfo object has an attribute of the same type as
        /// the given generic parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(ParameterInfo obj)
            where T : Attribute
        {
            return obj.IsDefined(typeof(T), false);
        }

        /// <summary>
        /// Check if the given type, or any of it's base types, has a given attribute.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static bool HasInheritedAttribute(Type type, Type attribute)
        {
            return type.IsDefined(attribute, true);
        }

        /// <summary>
        /// Check if the given type, or any of it's base types, has a given attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasInheritedAttribute<T>(Type type)
            where T : Attribute
        {
            return type.IsDefined(typeof(T), true);
        }

        #endregion

        /// <summary>
        /// Invoke a generic method using reflection. This is useful when you need
        /// to call a generic method on an object when you only have a Type object
        /// as a generic parameter.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="genericParam"></param>
        /// <param name="parameters"></param>
        public static void InvokeGenericMethod(
            object obj, string methodName, Type genericParam, BindingFlags bindingFlags,
            params object[] parameters)
        {
            var method = obj.GetType().GetMethod(methodName, bindingFlags);
            var generic = method.MakeGenericMethod(genericParam);
            generic.Invoke(obj, parameters);
        }

        /// <summary>
        /// Return the number of items defined in an enum.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int EnumItemCount(Type type)
        {
            return Enum.GetNames(type).Length;
        }

        /// <summary>
        /// Return all the items in an enum as a list.
        /// WARNING: This method isn't very fast, use with care!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> EnumItems<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Generic parameter must be an enum.");
            }
            return new List<T>((T[])Enum.GetValues(typeof(T)));   
        }

        /// <summary>
        /// Get the value of a property with the given name from an object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object GetPropertyValue(object obj, string name)
        {
            return obj.GetType().GetProperty(name).GetValue(obj, null);
        }

        /// <summary>
        /// Get the value of a property with the given name and type from an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(object obj, string name)
        {
            return (T)GetPropertyValue(obj, name);
        }

        /// <summary>
        /// Get the value of a nested property with the given URI from an object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="nameParts"></param>
        /// <returns></returns>
        public static object GetNestedPropertyValue(object obj, string[] nameParts)
        {
            string crntName;
            while (nameParts.Length > 0)
            {
                crntName = nameParts[0];
                obj = GetPropertyValue(obj, crntName);
                nameParts = nameParts.Skip(1).ToArray();
            }
            return obj;
        }

        /// <summary>
        /// Get the value of a nested property with the given URI from an object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object GetNestedPropertyValue(object obj, string name)
        {
            return GetNestedPropertyValue(obj, UriUtilities.GetParts(name));
        }

        /// <summary>
        /// Get the value of a nested property with the given URI and type from an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetNestedProperty<T>(object obj, string name)
        {
            return (T)GetNestedPropertyValue(obj, name);
        }
    }
}
