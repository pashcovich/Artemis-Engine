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
    public class Effector<T> : TimeableObject, IEffector
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
        /// The function that retrieves the next value of the effected
        /// field given the elapsed time and the elapsed frames as input.
        /// </summary>
        private Func<double, int, T> Func;

        /// <summary>
        /// The value type returned by the effector function.
        /// </summary>
        private EffectorValueType ValueType;

        /// <summary>
        /// The operator relating the previous value returned by Func and
        /// the next value. By default this is EffectorOperatorType.InPlace,
        /// so the next value simply replaces the previous value.
        /// </summary>
        private EffectorOperator<T> Op;

        /// <summary>
        /// The previous value returned by Func.
        /// </summary>
        private T Prev;

        /// <summary>
        /// A dictionary mapping from ArtemisObject instances to their initial values.
        /// This is to avoid the issue of reusable effectors with ValueType equal to
        /// RelativeToStart not using the same initial value for multiple instances with
        /// potentially different initial values.
        /// </summary>
        protected readonly Dictionary<EffectableArtemisObject, T> InitialValues
            = new Dictionary<EffectableArtemisObject, T>();

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

        private bool inPlace_and_relativeToStart;

        public Effector( string fieldName
                       , string effectorName
                       , Func<double, int, T> func
                       , bool reusable = false )
            : this(fieldName, effectorName, func, EffectorValueType.Absolute, EffectorOperatorType.InPlace, reusable) { }

        public Effector( string fieldName
                       , string effectorName
                       , Func<double, int, T> func
                       , EffectorValueType valueType
                       , EffectorOperatorType opType
                       , bool reusable = false )
            : this(fieldName, effectorName, func, valueType, new EffectorOperator<T>(opType), reusable) { }

        public Effector( string fieldName
                       , string effectorName
                       , Func<double, int, T> func
                       , EffectorValueType valueType
                       , Func<T, T, T> op
                       , bool reusable = false )
            : this(fieldName, effectorName, func, valueType, new EffectorOperator<T>(op), reusable) { }
        
        public Effector( string fieldName
                       , string effectorName
                       , Func<double, int, T> func
                       , EffectorValueType valueType
                       , EffectorOperator<T> op
                       , bool reusable = false )
            : base()
        {
            EffectedFieldName = fieldName;
            EffectorName      = effectorName;
            ValueType         = valueType;
            Func              = func;
            Op                = op;

            Initialized = false;
            Reusable = reusable;

            inPlace_and_relativeToStart = 
                op.InternalType.HasValue && 
                op.InternalType.Value == EffectorOperatorType.InPlace && 
                valueType == EffectorValueType.RelativeToStart;
        }

        // NOTE: The only reason we have InternalInitiailize AND Initialize is 
        // because Initialize is the method that can be overridden in base classes,
        // whereas InternalInitialize simply calls Initialize, but is accessible
        // from within this assembly. This makes it so that both the user can
        // override Initialize, and the Assembly can call Initialize, without
        // exposing the Initialize method at a higher visibility.

        internal void InternalInitialize(EffectableArtemisObject obj)
        {
            Initialize(obj);
        }

        /// <summary>
        /// Initialize the effector. This is done after the object is added
        /// to an EffectableArtemisObject.
        /// </summary>
        protected virtual void Initialize(EffectableArtemisObject obj) 
        {
            if (Initialized)
            {
                string exceptionMessage;
                if (Anonymous)
                {
                     exceptionMessage = String.Format(
                         "Anonymous effector of type '{0}' was initialized twice. The most " +
                         "likely cause for this is the same effector was added to multiple " +
                         "objects. Check to ensure you aren't reusing the same effector.", 
                         this.GetType());
                }
                else
                {
                    exceptionMessage = String.Format(
                        "Effector with name '{0}' and type '{1}' was initialized twice. The " +
                        "most likely cause for this is the same effector was added to multiple " +
                        "objects. Check to ensure you aren't reusing the same effector.",
                        EffectorName, this.GetType());
                }
                throw new EffectorException(exceptionMessage);
            }
            Initialized = true;

            if (ValueType == EffectorValueType.RelativeToStart)
            {
                InitialValues.Add(obj, obj.Fields.Get<T>(EffectedFieldName));
            }

            // If this effector is reusable, then it may be used for multiple
            // ArtemisObject instances, meaning we can't store an internal reference
            // to any single one.
            //
            // The advantage of using a non-reusable effector over a reusable one is
            // that the effector can initialize it's own properties based on it's
            // assigned instance.
            if (Reusable)
            {
                return;
            }

            Object = obj;
        }


        internal void UpdateEffector(EffectableArtemisObject obj)
        {
            UpdateTime();

            if (!Reusable)
            {
                obj = Object;
            }

            
            var next = Func(ElapsedTime, ElapsedFrames);
            if (Prev != null)
            {
                // Combined is just the value after performing the operation O(x, y)
                // where x is the previous value and y is the next value.
                var combined = Op.Operate(Prev, next);

                if (ValueType == EffectorValueType.RelativeToStart)
                {
                    var init = InitialValues[obj];
                    if (inPlace_and_relativeToStart)
                    {
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

                        combined = GenericOperators.Add<T>(init, combined);
                    }
                    else
                    {
                        combined = Op.Operate(init, combined);
                    }
                }
                obj.Fields.Set<T>(EffectedFieldName, combined);
            }
            else
            {
                obj.Fields.Set<T>(EffectedFieldName, next);
            }
            Prev = next;
        }
    }
}
