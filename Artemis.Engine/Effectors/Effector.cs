#region Using Statements

using Artemis.Engine.Utilities;

using System;
using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Effectors
{

    /// <summary>
    /// An Effector is an object which acts as a function, which can be attached to an
    /// EffectableArtemisObject instance. It then automatically updates a given field
    /// of that instance over time.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Effector<T> : CoerciveEffector<T, T>
    {

        // Because constructor inheritance is blasphemy.
        #region Constructors

        public Effector( string fieldName
                       , string effectorName
                       , Func<double, int, T> func
                       , EffectorOperatorType opType = EffectorOperatorType.InPlace
                       , EffectorValueType valueType = EffectorValueType.RelativeToStart
                       , bool reusable = false )
            : base(fieldName, effectorName, func, opType, valueType, reusable) { }

        public Effector( string fieldName
                       , string effectorName
                       , Func<double, int, T> func
                       , Func<T, T, T> op
                       , EffectorValueType valueType = EffectorValueType.RelativeToStart
                       , bool reusable = false )
            : base(fieldName, effectorName, func, op, valueType, reusable) { }

        public Effector( string fieldName
                       , string effectorName
                       , Func<double, int, T> func
                       , EffectorOperator<T> op
                       , EffectorValueType valueType = EffectorValueType.RelativeToStart
                       , bool reusable = false )
            : base(fieldName, effectorName, func, op, valueType, reusable) { }

        #endregion

        protected override T CoerceTo(T val)
        {
            return val;
        }

        protected override void AssignNextValue(T val)
        {
            Set(EffectedPropertyName, val);
        }
    }
}
