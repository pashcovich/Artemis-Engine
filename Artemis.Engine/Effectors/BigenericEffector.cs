#region Using Statements

using Artemis.Engine.Utilities;

using System;
using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Effectors
{
    /// <summary>
    /// A BigenericEffector is an abstract effector where the type of the effected 
    /// field and the type returned by the effector function itself differ.
    /// 
    /// There are two generic parameters, T1 and T2. T1 represents the type of the
    /// actual effected field, whereas T2 represents the type of the values returned
    /// by the supplied Func object.
    /// 
    /// When implementing this class, two methods must be implemented; ConvertToEffectable
    /// and ConvertAndAssign. ConvertToEffectable takes the field of type T1 and converts 
    /// it to a T2 object. This is used when recording the initial value of an effector, 
    /// since the initial value starts off as a T2 object and must be converted to a T1 
    /// to actual be used as an initial value.
    /// 
    /// The ConvertAndAssign method takes a DynamicFieldContainer `fields` and a `T2`
    /// object `nextVal`. This method must set the effected field in `fields` to the given
    /// `T2` value after converting it to a `T1` object.
    /// 
    /// The reason ConvertAndAssign is all one operation is for things like XCoordEffector
    /// to work, since it's function returns a double object that must then be assigned
    /// to the X property of a Vector2 object.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public abstract class BigenericEffector<T1, T2> : TimeableObject, IEffector
    {
        /// <summary>
        /// The name of the field we're effecting. This MUST be a field
        /// in the effected object's DynamicFieldContainer object.
        /// </summary>
        public string EffectedFieldName { get; private set; }

        /// <summary>
        /// The name of this effector.
        /// </summary>
        public string EffectorName { get; private set; }

        /// <summary>
        /// Whether or not this effector is anonymous (has a name or not).
        /// </summary>
        public bool Anonymous { get { return EffectorName == null; } }

        /// <summary>
        /// Whether or not Initialize has been called. Initialize gets called
        /// when this effector has been added to an EffectableArtemisObject.
        /// </summary>
        public bool Initialized { get; private set; }

        /// <summary>
        /// Whether or not this effector is reusable. A reusable effector
        /// is one that doesn't store a reference to the object it was added
        /// to, meaning the same effector instance can be added to multiple
        /// objects. 
        /// 
        /// This is useful when you want to add a common effector to many objects 
        /// without having to create tons of instances of the same effector.
        /// </summary>
        public bool Reusable { get; private set; }

        /// <summary>
        /// The object this effector has been added to.
        /// </summary>
        protected EffectableArtemisObject Object { get; private set; }

        /// <summary>
        /// A dictionary mapping from ArtemisObject instances to their initial values.
        /// This is to avoid the issue of reusable effectors with ValueType equal to
        /// RelativeToStart not using the same initial value for multiple instances with
        /// potentially different initial values.
        /// </summary>
        protected readonly Dictionary<EffectableArtemisObject, T2> InitialValues
            = new Dictionary<EffectableArtemisObject, T2>();

        private Func<double, int, T2> Func;
        private EffectorValueType ValueType;
        private EffectorOperator<T2> Op;
        private T2 Prev;
        private bool inPlace_and_relativeToStart;

        #region Alternative Constructors

        public BigenericEffector( string fieldName
                                , string effectorName
                                , Func<double, int, T2> func
                                , EffectorOperatorType opType = EffectorOperatorType.InPlace
                                , EffectorValueType valueType = EffectorValueType.RelativeToStart
                                , bool reusable = false )
            : this( fieldName
                  , effectorName
                  , func
                  , new EffectorOperator<T2>(opType)
                  , valueType
                  , reusable) { }

        public BigenericEffector( string fieldName
                                , string effectorName
                                , Func<double, int, T2> func
                                , Func<T2, T2, T2> op
                                , EffectorValueType valueType = EffectorValueType.RelativeToStart
                                , bool reusable = false )
            : this( fieldName
                  , effectorName
                  , func
                  , new EffectorOperator<T2>(op)
                  , valueType
                  , reusable) { }

        #endregion

        public BigenericEffector( string fieldName
                                , string effectorName
                                , Func<double, int, T2> func
                                , EffectorOperator<T2> op
                                , EffectorValueType valueType = EffectorValueType.RelativeToStart
                                , bool reusable = false )
            : base()
        {
            EffectedFieldName = fieldName;
            EffectorName = effectorName;
            Func = func;

            ValueType = valueType;
            Op = op;

            Initialized = false;
            Reusable = reusable;

            inPlace_and_relativeToStart = 
                op.InternalType.HasValue && 
                op.InternalType.Value == EffectorOperatorType.InPlace && 
                valueType == EffectorValueType.RelativeToStart;
        }

        /// <summary>
        /// Convert the effected field to a type usable by the effector.
        /// This method is essentially an explicit conversion operator from
        /// T1 to T2.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        internal abstract T2 ConvertToEffectable(T1 val);

        /// <summary>
        /// Convert the value returned by the effector into a type assignable to
        /// the effected field, and then perform the assignment.
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="nextVal"></param>
        internal abstract void ConvertAndAssign(DynamicFieldContainer fields, T2 nextVal);

        internal void InternalInitialize(EffectableArtemisObject obj)
        {
            if (Initialized)
            {
                string exceptionMessage;
                if (Anonymous)
                {
                    exceptionMessage = String.Format(
                        "Anonymous effector of type '{0}' was initialized twice. The most " +
                        "likely cause for this is the same effector was added to multiple " +
                        "objects. Check to ensure you aren't reusing the same effector instance.",
                        this.GetType());
                }
                else
                {
                    exceptionMessage = String.Format(
                        "Effector with name '{0}' and type '{1}' was initialized twice. The " +
                        "most likely cause for this is the same effector was added to multiple " +
                        "objects. Check to ensure you aren't reusing the same effector instance.",
                        EffectorName, this.GetType());
                }
                throw new EffectorException(exceptionMessage);
            }

            if (ValueType == EffectorValueType.RelativeToStart)
            {
                InitialValues.Add(obj, ConvertToEffectable(obj.Fields.Get<T1>(EffectedFieldName)));
            }

            // If this effector is reusable, then it may be used for multiple
            // ArtemisObject instances, meaning we can't store an internal reference
            // to any single one.
            //
            // The advantage of using a non-reusable effector over a reusable one is
            // that the effector can initialize it's own properties based on it's
            // assigned instance.
            if (!Reusable)
            {
                Object = obj;
            }

            Initialize();

            Initialized = true;
        }

        /// <summary>
        /// Initialize the effector. This is done after the object is added
        /// to an EffectableArtemisObject.
        /// </summary>
        protected virtual void Initialize() { }

        /// <summary>
        /// Return the next value of the effector.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual T2 GetNext(EffectableArtemisObject obj)
        {
            var next = Func(ElapsedTime, ElapsedFrames);
            if (Prev != null)
            {
                // Combined is just the value after performing the operation O(x, y)
                // where x is the previous value and y is the next value.
                var combined = Op.Operate(Prev, next);

                if (ValueType == EffectorValueType.RelativeToStart)
                {
                    var init = InitialValues[obj];

                    #region Explanation of why we must check if the effector is InPlace and RelativeToStart.

                    // Alright here's the breakdown. The following table outlines the
                    // general term for a given ValueType and OperatorType, where x0 is
                    // the initial value, F is Func, and tn and fn are the time and frame 
                    // counts at the nth call to UpdateEffector respectively:
                    //
                    // ______________________________________________________________
                    // |                |     RelativeToStart    |     Absolute     |
                    // |------------------------------------------------------------|
                    // | InPlace:       |     xn = F(tn, fn)     |   = F(tn, fn)    |
                    // |----------------|------------------------|------------------|
                    // | Additive:      |  xn = x0 + Σ F(tn, fn) |   = Σ F(tn, fn)  |
                    // |----------------|------------------------|------------------|
                    // | Multiplicative |  xn = x0 * Π F(tn, fn) |   = Π F(tn, fn)  |
                    // --------------------------------------------------------------
                    //
                    // Notice how for Inplace and RelativeToStart, the general term is
                    // F(tn, fn). Although this is technically correct and makes sense with
                    // respect to the rest of the values (xn = O(x(n - 1), F(tn, fn)) in general,
                    // where O is the operation, so for InPlace O(x, y) = y means xn = F(tn, fn)),
                    // it feels unnatural or unintuitive.
                    //
                    // Intuitively, it makes more sense that InPlace and RelativeToStart would mean
                    // the general term would be x0 + F(tn, fn), since that is really what "Relative
                    // To Start" means.
                    //
                    // Mechanically, it also makes sense that you would want InPlace and RelativeToStart
                    // to do something different from InPlace and Absolute. For example, if F(tn, fn) = tn,
                    // and x0 = 3, then you would expect the general term to be 3 + tn, not just tn.
                    // If InPlace and RelativeToStart didn't do that, then there would be no way of
                    // achieving the desired outcome (Additive and RelativeToStart would give 3 + Σ tn).
                    //
                    // Thus, if the effector is both InPlace and RelativeToStart, then the general
                    // term is actually xn = x0 + F(tn, fn), instead of the term predicted by the pattern
                    // xn = O(x(n - 1), F(tn, fn)).

                    #endregion

                    return inPlace_and_relativeToStart ? GenericOperators.Add<T2>(init, combined)
                                                       : Op.Operate(init, combined);
                }
                return combined;
            }
            return next;
        }

        public void UpdateEffector(EffectableArtemisObject obj)
        {
            UpdateTime();

            if (!Reusable)
            {
                obj = Object;
            }

            var next = GetNext(obj);
            Prev = next;

            ConvertAndAssign(obj.Fields, next);
        }
    }
}
