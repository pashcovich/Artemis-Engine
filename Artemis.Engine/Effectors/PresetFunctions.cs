#region Using Statements

using System;

#endregion

namespace Artemis.Engine.Effectors
{

    /// <summary>
    /// A static class of preset functions that can be used as the Func argument
    /// in an effector.
    /// 
    /// Definition: An "effector function" is a Func<double, int, T> (T is arbitrary)
    /// that can be supplied to an effector.
    /// </summary>
    public static class PresetFunctions
    {

        /// <summary>
        /// Return an effector function scaled by it's time argument.
        /// 
        /// If the given function is (t, f) => F(t, f), then the resulting
        /// function returned will be (t, f) => A*F(B*t + C, f) + D.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <returns></returns>
        public static Func<double, int, double> ScaleTime(
            Func<double, int, double> func, double A, double B, double C, double D)
        {
            return (t, f) => A*func(B*t + C, f) + D;
        }

        /// <summary>
        /// Return an effector function scaled by it's frames argument.
        /// 
        /// If the given function is (t, f) => F(t, f), then the resulting
        /// function returned will be (t, f) => A*F(t, B*f + C) + D.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <returns></returns>
        public static Func<double, int, double> ScaleFrames(
            Func<double, int, double> func, double A, int B, int C, double D)
        {
            return (t, f) => A*func(t, B*f + C) + D;
        }

        /// <summary>
        /// Return a sin effector function scaled by it's time argument.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <returns></returns>
        public static Func<double, int, double> ScaledSin(
            double A, double B, double C, double D)
        {
            return ScaleTime((t, f) => Math.Sin(t), A, B, C, D);
        }

        /// <summary>
        /// Return a sin effector function scaled by it's frame argument.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <returns></returns>
        public static Func<double, int, double> ScaledSin(
            double A, int B, int C, double D)
        {
            return ScaleFrames((t, f) => Math.Sin(f), A, B, C, D);
        }
    }
}
