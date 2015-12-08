using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public static List<Attribute> GetAttributes(Assembly obj, Type attributeType)
        {
            return _GetAttributes(Attribute.GetCustomAttributes(obj), attributeType);
        }

        public static List<Attribute> GetAttributes(MemberInfo obj, Type attributeType)
        {
            return _GetAttributes(Attribute.GetCustomAttributes(obj), attributeType);
        }

        public static List<Attribute> GetAttributes(Module obj, Type attributeType)
        {
            return _GetAttributes(Attribute.GetCustomAttributes(obj), attributeType);
        }

        public static List<Attribute> GetAttributes(ParameterInfo obj, Type attributeType)
        {
            return _GetAttributes(Attribute.GetCustomAttributes(obj), attributeType);
        }

        public static List<T> GetAttributes<T>(Assembly obj)
            where T : Attribute
        {
            return _GetAttributes<T>(Attribute.GetCustomAttributes(obj));
        }

        public static List<T> GetAttributes<T>(MemberInfo obj)
            where T : Attribute
        {
            return _GetAttributes<T>(Attribute.GetCustomAttributes(obj));
        }

        public static List<T> GetAttributes<T>(Module obj)
            where T : Attribute
        {
            return _GetAttributes<T>(Attribute.GetCustomAttributes(obj));
        }

        public static List<T> GetAttributes<T>(ParameterInfo obj)
            where T : Attribute
        {
            return _GetAttributes<T>(Attribute.GetCustomAttributes(obj));
        }

        public static Attribute GetFirstAttribute(Assembly obj, Type attributeType)
        {
            return GetAttributes(obj, attributeType).FirstOrDefault();
        }

        public static Attribute GetFirstAttribute(MemberInfo obj, Type attributeType)
        {
            return GetAttributes(obj, attributeType).FirstOrDefault();
        }

        public static Attribute GetFirstAttribute(Module obj, Type attributeType)
        {
            return GetAttributes(obj, attributeType).FirstOrDefault();
        }

        public static Attribute GetFirstAttribute(ParameterInfo obj, Type attributeType)
        {
            return GetAttributes(obj, attributeType).FirstOrDefault();
        }

        public static T GetFirstAttribute<T>(Assembly obj)
            where T : Attribute
        {
            return GetAttributes<T>(obj).FirstOrDefault();
        }

        public static T GetFirstAttribute<T>(MemberInfo obj)
            where T : Attribute
        {
            return GetAttributes<T>(obj).FirstOrDefault();
        }

        public static T GetFirstAttribute<T>(Module obj)
            where T : Attribute
        {
            return GetAttributes<T>(obj).FirstOrDefault();
        }

        public static T GetFirstAttribute<T>(ParameterInfo obj)
            where T : Attribute
        {
            return GetAttributes<T>(obj).FirstOrDefault();
        }

        public static bool HasAttribute(Assembly obj, Type attributeType)
        {
            return obj.IsDefined(attributeType, false);
        }

        public static bool HasAttribute(MemberInfo obj, Type attributeType)
        {
            return obj.IsDefined(attributeType, false);
        }

        public static bool HasAttribute(Module obj, Type attributeType)
        {
            return obj.IsDefined(attributeType, false);
        }

        public static bool HasAttribute(ParameterInfo obj, Type attributeType)
        {
            return obj.IsDefined(attributeType, false);
        }

        public static bool HasAttribute<T>(Assembly obj)
            where T : Attribute
        {
            return obj.IsDefined(typeof(T), false);
        }

        public static bool HasAttribute<T>(MemberInfo obj)
            where T : Attribute
        {
            return obj.IsDefined(typeof(T), false);
        }

        public static bool HasAttribute<T>(Module obj)
            where T : Attribute
        {
            return obj.IsDefined(typeof(T), false);
        }

        public static bool HasAttribute<T>(ParameterInfo obj)
            where T : Attribute
        {
            return obj.IsDefined(typeof(T), false);
        }

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
    }
}
