#region Using Statements

using Artemis.Engine.Utilities;

using System;

#endregion

namespace Artemis.Engine.Effectors
{
    /// <summary>
    /// A class which represents an operation that an Effector uses
    /// to combine it's previous and next returned values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EffectorOperator<T>
    {

        /// <summary>
        /// The actual operation.
        /// </summary>
        public Func<T, T, T> Operate { get; private set; }

        /// <summary>
        /// The internal type of the operation (if it was instantiated using an
        /// EffectorOperatorType enum item).
        /// </summary>
        public EffectorOperatorType? InternalType { get; private set; }

        internal EffectorOperator(EffectorOperatorType type)
        {
            InternalType = type;
            switch (type)
            {
                case EffectorOperatorType.InPlace:
                    Operate = (a, b) => b;
                    break;
                case EffectorOperatorType.Additive:
                    Operate = GenericOperators.Add<T>;
                    break;
                case EffectorOperatorType.Multiplicative:
                    Operate = GenericOperators.Mul<T>;
                    break;
                case EffectorOperatorType.ArithmeticAveraging:
                    Operate = GenericOperators.Average<T>;
                    break;
                default:
                    throw new EffectorOperatorException(
                        String.Format(
                            "Unknown EffectorOperatorType supplied: '{0}'.", type
                            )
                        );
            }
        }

        internal EffectorOperator(Func<T, T, T> operate)
        {
            Operate = operate;
        }
    }
}
