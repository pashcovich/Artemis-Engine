#region Using Statements

using System;

#endregion

namespace Artemis.Engine.Effectors.Special
{
    /// <summary>
    /// An effector that acts on integers.
    /// </summary>
    public class IntEffector : Effector<int> 
    {

        #region Constructors

        public IntEffector( string fieldName
                          , string effectorName
                          , Func<double, int, int> func
                          , EffectorOperatorType opType = EffectorOperatorType.InPlace
                          , EffectorValueType valueType = EffectorValueType.RelativeToStart
                          , bool reusable = false )
            : base(fieldName, effectorName, func, opType, valueType, reusable) { }

        public IntEffector( string fieldName
                          , string effectorName
                          , Func<double, int, int> func
                          , Func<int, int, int> op
                          , EffectorValueType valueType = EffectorValueType.RelativeToStart
                          , bool reusable = false )
            : base(fieldName, effectorName, func, op, valueType, reusable) { }

        public IntEffector( string fieldName
                          , string effectorName
                          , Func<double, int, int> func
                          , EffectorOperator<int> op
                          , EffectorValueType valueType = EffectorValueType.RelativeToStart
                          , bool reusable = false )
            : base(fieldName, effectorName, func, op, valueType, reusable) { }

        #endregion

        protected override int Combine_InPlaceAndRelativeToStart(int init, int combined)
        {
            return init + combined;
        }

    }
}
