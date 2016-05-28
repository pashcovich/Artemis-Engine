#region Using Statements

using Artemis.Engine.Utilities;
using Artemis.Engine.Utilities.Partial;

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Artemis.Engine
{

    /// <summary>
    /// A TimeableObject is an object that maintains the time it has been alive for, 
    /// and exposing methods that measure the current position on the timeline of the 
    /// object's life (for example, AtFrame determines if the current number of elapsed
    /// frames the object has been alive for is equal to a given number of frames).
    /// </summary>
    public class TimeableObject : UpdatableObject
    {
        private struct TimedInvocation
        {
            /// <summary>
            /// Whether or not this invocation is based on elapsed frames or elapsed time.
            /// </summary>
            public bool UsingFrames;

            /// <summary>
            /// The amount of frames to pass before invocation.
            /// </summary>
            public int Frames;

            /// <summary>
            /// The frame this invocation was created.
            /// </summary>
            public int StartFrame;

            /// <summary>
            /// The amount of time to pass before invocation.
            /// </summary>
            public double Milliseconds;

            /// <summary>
            /// The start time of this invocation.
            /// </summary>
            public double StartTime;

            /// <summary>
            /// The action to invoke.
            /// </summary>
            public Action Action;
        }

        /// <summary>
        /// The default value of ArtemisEngine.GameTimer.DeltaTime, used when running in
        /// a partial engine environment.
        /// </summary>
        private const double DEFAULT_DELTA_TIME = 16.66666666666;

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

        private HashSet<TimedInvocation> timedInvocations;

        // TODO: Use SkipLists here for O(1) retrieval of first element (SortedDictionaries are O(log(n)).
        // Don't want to use SortedList because they have average-case O(n) insertion.
        private SortedDictionary<int, TimedInvocation> _fastTimedInvocations_frames;
        private SortedDictionary<double, TimedInvocation> _fastTimedInvocations_milliseconds;

        /// <summary>
        /// Whether or not this object can handle heavy loads of timed invocations. Set this to true
        /// if your object uses a very large number of timed invocations.
        /// </summary>
        public bool UseHeavyInvocationLoadHandling;

        private Action UpdateTime { get; set; }

        public TimeableObject() : base()
        {
            ResetTime();

            // This is probably overkill...
            //  
            // We assign a different updater function depending on the partial
            // state of the engine so that we're not making the check for IsPartial
            // every time UpdateTime is called (which is a lot).
            if (!IsPartial)
            {
                UpdateTime = updateTime;
            }
            else
            {
                UpdateTime = updateTime_Partial;
            }

            timedInvocations = new HashSet<TimedInvocation>();
            // _fastTimedInvocations_frames = new SortedDictionary<int, TimedInvocation>();
            // _fastTimedInvocations_milliseconds = new SortedDictionary<double, TimedInvocation>();
        }

        private void updateTime()
        {
            PrevElapsedTime = ElapsedTime;
            ElapsedTime += ArtemisEngine.GameTimer.DeltaTime;
            ElapsedFrames++;
        }

        private void updateTime_Partial()
        {
            PrevElapsedTime = ElapsedTime;
            ElapsedTime += DEFAULT_DELTA_TIME;
            ElapsedFrames++;
        }

        internal override void InternalUpdate()
        {
            base.InternalUpdate();

            UpdateTime();

            if (UseHeavyInvocationLoadHandling)
            {
                throw new NotImplementedException(
                    "Support for heavy invocation load handling is not yet implemented.");
            }
            else
            {
                var toRemove = new List<TimedInvocation>();
                foreach (var invocation in timedInvocations)
                {
                    if ((invocation.UsingFrames && ElapsedFrames - invocation.StartFrame >= invocation.Frames) ||
                        (ElapsedTime - invocation.StartTime >= invocation.Milliseconds))
                    {
                        invocation.Action();
                        toRemove.Add(invocation);
                    }
                }

                foreach (var invocation in toRemove)
                {
                    timedInvocations.Remove(invocation);
                }
            }
        }

        public void ResetTime(bool clearInvocations = false)
        {
            ElapsedFrames   = 0;
            ElapsedTime     = 0;
            PrevElapsedTime = 0;

            if (clearInvocations)
                timedInvocations.Clear();
        }

        /// <summary>
        /// Invoke a given action after the given number of frames have passed.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="frames"></param>
        public void InvokeAfter(Action action, int frames)
        {
            if (frames < 0)
                return;
            timedInvocations.Add(new TimedInvocation
            {
                Action = action,
                Frames = (int)frames,
                StartFrame = ElapsedFrames,
                UsingFrames = true
            });
        }

        /// <summary>
        /// Invoke a given action after the given number of milliseconds have passed.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="time"></param>
        public void InvokeAfter(Action action, double time)
        {
            if (time < 0)
                return;
            timedInvocations.Add(new TimedInvocation
            {
                Action = action,
                Milliseconds = time,
                StartTime = ElapsedTime
            });
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
        /// Returns true if the elapsed time is between the given start and
        /// end times, and is congruent to zero mod the given interval (with an error
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

        /// <summary>
        /// Periodically alternates between being true for the given interval of time,
        /// and then false for the next interval of time, continuing indefinitely.
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool DuringIntervals(int interval, int start = 0, int end = Int32.MaxValue)
        {
            return DuringOrAt(start, end) && (ElapsedFrames - start) % (2 * interval) < interval;
        }

        /// <summary>
        /// Periodically alternates between being true for the given interval of time,
        /// and then false for the next interval of time, continuing indefinitely.
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool DuringIntervals(double interval, double start = 0, double end = Double.PositiveInfinity)
        {
            return DuringOrAt(start, end) && (ElapsedTime - start) % (2 * interval) < interval;
        }
    }
}
