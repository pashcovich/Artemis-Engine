#region Using Statements

using System;

#endregion

namespace Artemis.Engine
{

    /// <summary>
    /// A TimeableObject is an object that maintains the time it has been alive for, 
    /// and exposing methods that measure the current position on the timeline of the 
    /// object's life (for example, AtFrame determines if the current number of elapsed
    /// frames the object has been alive for is equal to a given number of frames).
    /// </summary>
    public class TimeableObject
    {

        /// <summary>
        /// Decides whether or not to update object
        /// </summary>
        public bool NeedsUpdate { get; internal set; }

        /// <summary>
        /// The previous elapsed time in milliseconds.
        /// </summary>
        public double PrevElapsedTime { get; private set; }

        /// <summary>
        /// The amount of time in milliseconds elapsed since this object was created.
        /// </summary>
        public double ElapsedTime { get; private set; }

        /// <summary>
        /// The number of frames this object has been alive for.
        /// </summary>
        public int ElapsedFrames { get; private set; }

        public TimeableObject()
        {
            ElapsedFrames   = 0;
            ElapsedTime     = 0;
            PrevElapsedTime = 0;
        }

        protected void UpdateTime()
        {
            PrevElapsedTime = ElapsedTime;
            ElapsedTime += ArtemisEngine.GameTimer.DeltaTime;
            ElapsedFrames++;

            NeedsUpdate = false;
        }

        /// <summary>
        /// Returns true if the elapsed frame count is equal to the given frame.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool At(int frame)
        {
            return ElapsedFrames == frame;
        }

        /// <summary>
        /// Returns true if the elapsed time is equal to the given time (with an 
        /// error margin equal to <code>ArtemisEngine.GameTimer.DeltaTime</code>).
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool At(double time)
        {
            return PrevElapsedTime <= time && time <= ElapsedTime;
        }

        /// <summary>
        /// Returns true if the elapsed frame count is less than the given frame.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool Before(int frame)
        {
            return ElapsedFrames < frame;
        }

        /// <summary>
        /// Returns true if the elapsed time is less than the given time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool Before(double time)
        {
            return ElapsedTime < time;
        }

        /// <summary>
        /// Returns true if the elapsed frame count is less than or equal to 
        /// the given frame.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool BeforeOrAt(int frame)
        {
            return ElapsedFrames <= frame;
        }

        /// <summary>
        /// Returns true if the elapsed time is less than or equal to the given time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool BeforeOrAt(double time)
        {
            return ElapsedTime <= time;
        }

        /// <summary>
        /// Returns true if the elapsed frame count is greater than the given frame.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool After(int frame)
        {
            return ElapsedFrames > frame;
        }

        /// <summary>
        /// Returns true if the elapsed time is greater than the given time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool After(double time)
        {
            return ElapsedTime > time;
        }

        /// <summary>
        /// Returns true if the elapsed frame count is greater than or equal
        /// to the given frame.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool AfterOrAt(int frame)
        {
            return ElapsedFrames >= frame;
        }

        /// <summary>
        /// Returns true if the elapsed time is greater than or equal to the
        /// given time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool AfterOrAt(double time)
        {
            return ElapsedTime >= time;
        }

        /// <summary>
        /// Returns true if the elapsed frame count is strictly between the given
        /// start and end frames.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool During(int start, int end)
        {
            return start < ElapsedFrames && ElapsedFrames < end;
        }

        /// <summary>
        /// Returns true if the elapsed time is strictly between the given start
        /// and end times.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool During(double start, double end)
        {
            return start < ElapsedTime && ElapsedTime < end;
        }

        /// <summary>
        /// Returns true if the elapsed frame count is between the given start
        /// and end frames inclusive.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool DuringOrAt(int start, int end)
        {
            return start <= ElapsedFrames && ElapsedFrames <= end;
        }

        /// <summary>
        /// Returns true if the elapsed time is between the given start and
        /// end times inclusive.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool DuringOrAt(double start, double end)
        {
            return start <= ElapsedTime && ElapsedTime <= end;
        }

        /// <summary>
        /// Returns true if the elapsed frame count is strictly outside the given
        /// start and end frame interval.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool Outside(int start, int end)
        {
            return !DuringOrAt(start, end);
        }

        /// <summary>
        /// Returns true if the elapsed time is strictly outside the given start
        /// and end time interval.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool Outside(double start, double end)
        {
            return !DuringOrAt(start, end);
        }

        /// <summary>
        /// Returns true if the elapsed frame count is outisde the given start
        /// and end frame interval inclusive.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool OutsideOrAt(int start, int end)
        {
            return !During(start, end);
        }

        /// <summary>
        /// Returns true if the elapsed time is outside the given start and end
        /// time interval inclusive.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool OutsideOrAt(double start, double end)
        {
            return !During(start, end);
        }

        /// <summary>
        /// Returns true if the elapsed frame count is between the given start and
        /// end frames, and is congruent to zero mod the given interval.
        /// 
        /// If EF is the number of elapsed frames, then this function is equivalent
        /// to saying (start <= EF && EF <= end) && (EF - start) % interval == 0.
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool AtIntervals(int interval, int start = 0, int end = Int32.MaxValue)
        {
            return DuringOrAt(start, end) && (ElapsedFrames - start) % interval == 0;
        }

        /// <summary>
        /// Returns true if the elapsed frame count is between the given start and
        /// end frames, and is congruent to zero mod the given interval (with an error
        /// equal to <code>ArtemisEngine.GameTimer.DeltaTime</code>).
        /// 
        /// If ET is the elapsed time, then this function is equivalent to saying
        /// <code>(start <= ET && ET <= end) && (ET - start) % interval < ArtemisEngine.GameTimer.DeltaTime</code>.
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool AtIntervals(double interval, double start = 0, double end = Double.PositiveInfinity)
        {
            return DuringOrAt(start, end) && (ElapsedTime - start) % interval < ArtemisEngine.GameTimer.DeltaTime;
        }

        public bool DuringIntervals(int interval, int start = 0, int end = Int32.MaxValue)
        {
            return DuringOrAt(start, end) && (ElapsedFrames - start) % (2 * interval) < interval;
        }

        public bool DuringIntervals(double interval, double start = 0, double end = Double.PositiveInfinity)
        {
            return DuringOrAt(start, end) && (ElapsedTime - start) % (2 * interval) < interval;
        }
    }
}
