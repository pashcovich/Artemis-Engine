#region Using Statement

using Microsoft.Xna.Framework;

using System;

#endregion

namespace Artemis.Engine.Effectors.Special
{
    /// <summary>
    /// An effector that acts on the y-component of a Vector2 object.
    /// </summary>
    public class YCoordEffector : BigenericEffector<Vector2, float>
    {
        #region Constructors

        public YCoordEffector( string fieldName
                             , string effectorName
                             , Func<double, int, float> func
                             , EffectorOperatorType opType = EffectorOperatorType.InPlace
                             , EffectorValueType valueType = EffectorValueType.RelativeToStart
                             , bool reusable = false )
            : base(fieldName, effectorName, func, opType, valueType, reusable) { }

        public YCoordEffector( string fieldName
                             , string effectorName
                             , Func<double, int, float> func
                             , Func<float, float, float> op
                             , EffectorValueType valueType = EffectorValueType.RelativeToStart
                             , bool reusable = false )
            : base(fieldName, effectorName, func, op, valueType, reusable) { }

        public YCoordEffector( string fieldName
                             , string effectorName
                             , Func<double, int, float> func
                             , EffectorOperator<float> op
                             , EffectorValueType valueType = EffectorValueType.RelativeToStart
                             , bool reusable = false )
            : base(fieldName, effectorName, func, op, valueType, reusable) { }

        #endregion

        internal override float ConvertToEffectable(Vector2 val)
        {
            return val.Y;
        }

        internal override void ConvertAndAssign(DynamicFieldContainer fields, float nextVal)
        {
            var prevVec = fields.Get<Vector2>(EffectedFieldName);
            prevVec.Y = nextVal;
            fields.Set<Vector2>(EffectedFieldName, prevVec);
        }
    }
}
