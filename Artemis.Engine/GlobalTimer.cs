#region Using Statements

using Microsoft.Xna.Framework;

using System.Collections.Generic;

#endregion

namespace Artemis.Engine
{
    public sealed class GlobalTimer
    {
        /// <summary>
        /// Global reference to the games total time and it's propeties
        /// </summary>
        public GameTime GlobalGameTime { get; private set; }

        /// <summary>
        /// Total time in milliseconds
        /// </summary>
        public double ElapsedTime { get; private set; }

        /// <summary>
        /// Total frames advanced
        /// </summary>
        public int ElapsedFrames { get; private set; }

        /// <summary>
        /// Time change between updates
        /// </summary>
        public double DeltaTime { get; private set; }

        // HashSet because it has fast insertion, fast removal, and fast iteration, which is
        // all we need.
        private HashSet<TimeableObject> TimeableObjects = new HashSet<TimeableObject>();

        internal GlobalTimer() { }

        internal void AddTimeableObject(TimeableObject obj)
        {
            TimeableObjects.Add(obj);
        }

        internal void RemoveTimeableObject(TimeableObject obj)
        {
            TimeableObjects.Remove(obj);
        }

        /// <summary>
        /// Updates total game time with new time
        /// </summary>
        internal void UpdateTime(GameTime gameTime)
        {
            DeltaTime = gameTime.ElapsedGameTime.TotalMilliseconds;
            GlobalGameTime = gameTime;
            ElapsedTime += DeltaTime;
            ElapsedFrames++;

            foreach (var obj in TimeableObjects)
            {
                obj.UpdateTime();
            }
        }
    }
}
