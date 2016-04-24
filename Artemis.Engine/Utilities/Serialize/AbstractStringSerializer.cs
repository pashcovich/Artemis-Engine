#region Using Statements

using Microsoft.Xna.Framework;
using System;

#endregion

namespace Artemis.Engine.Utilities.Serialize
{

    /// <summary>
    /// A serializer meant to convert an object to and from a string.
    /// 
    /// This is meant to replace an object's "ToString" method as this also
    /// provides an accompanying inverse "FromString" method.
    /// </summary>
    public abstract class AbstractStringSerializer
    {
        /// <summary>
        /// Convert an object to a string.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract string ToString(object obj);

        /// <summary>
        /// Convert a string to an object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract object FromString(string value);
    }

    /// <summary>
    /// A StringSerializer who's "ToString" is simply the object's "ToString"  method.
    /// </summary>
    public abstract class PartialStringSerializer : AbstractStringSerializer
    {
        /// <summary>
        /// Convert a string to an object.
        /// 
        /// Note: this should be the inverse operation to the object's "ToString" method.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override string ToString(object obj)
        {
            return obj.ToString();
        }
    }

    /// <summary>
    /// A StringSerializer for Int32s.
    /// </summary>
    public class Int32StringSerializer : PartialStringSerializer
    {
        public override object FromString(string value)
        {
            return Convert.ToInt32(value);
        }
    }

    /// <summary>
    /// A StringSerializer for floats.
    /// </summary>
    public class FloatStringSerializer : PartialStringSerializer
    {
        public override object FromString(string value)
        {
            return Convert.ToSingle(value);
        }
    }

    /// <summary>
    /// A StringSerializer for doubles.
    /// </summary>
    public class DoubleStringSerializer : PartialStringSerializer
    {
        public override object FromString(string value)
        {
            return Convert.ToDouble(value);
        }
    }

    /// <summary>
    /// A StringSerializer for booleans.
    /// </summary>
    public class BoolStringSerializer : PartialStringSerializer
    {
        public override object FromString(string value)
        {
            return Convert.ToBoolean(value);
        }
    }

    /// <summary>
    /// A StringSerializer for Vector2s.
    /// </summary>
    public class Vector2StringSerializer : AbstractStringSerializer
    {
        public override string ToString(object obj)
        {
            var vec = (Vector2)obj;
            return vec.X.ToString() + "," + vec.Y.ToString();
        }

        public override object FromString(string value)
        {
            var parts = value.Split(',');
            return new Vector2(Convert.ToSingle(parts[0]), Convert.ToSingle(parts[1]));
        }
    }

    /// <summary>
    /// A StringSerializer for Resolutions.
    /// </summary>
    public class ResolutionStringSerializer : AbstractStringSerializer
    {
        public override string ToString(object obj)
        {
            var vec = (Resolution)obj;
            return vec.Width.ToString() + "," + vec.Height.ToString();
        }

        public override object FromString(string value)
        {
            var parts = value.Split(',');
            return new Resolution(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]));
        }
    }
}
