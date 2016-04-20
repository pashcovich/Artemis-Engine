#region Using Statements

using System;

#endregion

namespace Artemis.Engine.Effectors.Special
{
    /// <summary>
    /// An effector that acts on doubles.
    /// </summary>
    public class DoubleEffector : Effector<double> 
    {
        #region Constructors

        public DoubleEffector( string fieldName
                             , string effectorName
                             , EffectorFunction<double> func
                             , EffectorOperatorType opType = EffectorOperatorType.InPlace
                             , EffectorValueType valueType = EffectorValueType.RelativeToStart
                             , bool reusable = false )
            : base(fieldName, effectorName, func, opType, valueType, reusable) { }

        public DoubleEffector( string fieldName
                             , string effectorName
                             , EffectorFunction<double> func
                             , Func<double, double, double> op
                             , EffectorValueType valueType = EffectorValueType.RelativeToStart
                             , bool reusable = false )
            : base(fieldName, effectorName, func, op, valueType, reusable) { }

        public DoubleEffector( string fieldName
                             , string effectorName
                             , EffectorFunction<double> func
                             , EffectorOperator<double> op
                             , EffectorValueType valueType = EffectorValueType.RelativeToStart
                             , bool reusable = false )
            : base(fieldName, effectorName, func, op, valueType, reusable) { }

        #endregion

        protected override double Combine_InPlaceAndRelativeToStart(double init, double combined)
        {
            return init + combined;
        }
    }
}
