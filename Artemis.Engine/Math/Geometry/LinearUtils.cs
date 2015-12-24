#region Using Statements

using Microsoft.Xna.Framework;

#endregion

namespace Phosphaze.Framework.Maths.Geometry
{
    public static class LinearUtils
    {

        /// <summary>
        /// Check if a point is in the "range" of a segment.
        /// 
        /// This essentially checks if the point is within the segment's bounding box.
        /// </summary>
        public static bool IsPointInSegmentRange(
            double px, double py,
            double x1, double y1,
            double x2, double y2)
        {
            if (x1 == x2)
                return (y1 <= py) == (py <= y2);
            return (x1 <= px) == (px <= x2);
        }

        /// <summary>
        /// Check if a point is in the "range" of a segment.
        /// 
        /// This essentially checks if the point is within the segment's bounding box.
        /// </summary>
        public static bool IsPointInSegmentRange(Vector2 point, Vector2 start, Vector2 end)
        {
            return IsPointInSegmentRange(point.X, point.Y, start.X, start.Y, end.X, end.Y);
        }

        /// <summary>
        /// Calculate the intersection point between two lines (or null if they are parallel).
        /// </summary>
        public static Vector2? LineIntersectionPoint(
            double x1, double y1, double x2, double y2,
            double x3, double y3, double x4, double y4)
        {
            if (((x1 == x2) && (x3 == x4)) || ((y1 == y2) && (y3 == y4)))
                return null;
            else if (x1 == x2)
            {
                double m1 = (y4 - y3) / (x4 - x3);
                double b1 = y3 - m1 * x3;
                double py = m1 * x1 + b1;
                return new Vector2((float)x1, (float)py);
            }
            else if (x3 == x4)
            {
                double m1 = (y2 - y1) / (x2 - x1);
                double b1 = y1 - m1 * x1;
                double py = m1 * x3 + b1;
                return new Vector2((float)x3, (float)py);
            }
            else
            {
                double m1 = (y2 - y1) / (x2 - x1);
                double m2 = (y4 - y3) / (x4 - x3);

                if (m1 == m2)
                    return null;

                double b1 = y1 - m1 * x1;
                double b2 = y3 - m2 * x3;

                double px = (b2 - b1) / (m1 - m2);
                double py = m1 * px + b1;

                return new Vector2((float)px, (float)py);
            }
        }

        /// <summary>
        /// Calculate the intersection point between two lines (or null if they are parallel).
        /// </summary>
        public static Vector2? LineIntersectionPoint(
            Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            return LineIntersectionPoint(
                start1.X, start1.Y, end1.X, end1.Y, start2.X, start2.Y, end2.X, end2.Y);
        }

        /// <summary>
        /// Calculate the intersection point between a line and a line segment (or null if they
        /// do not intersect).
        /// </summary>
        public static Vector2? LineToSegmentIntersectionPoint(
            double x1, double y1, double x2, double y2,
            double x3, double y3, double x4, double y4)
        {
            var p = LineIntersectionPoint(x1, y1, x2, y2, x3, y3, x4, y4);
            if (p.HasValue && IsPointInSegmentRange(p.Value.X, p.Value.Y, x3, y3, x4, y4))
                return p;
            return null;
        }

        /// <summary>
        /// Calculate the intersection point between a line and a line segment (or null if they
        /// do not intersect).
        /// </summary>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <param name="segStart"></param>
        /// <param name="segEnd"></param>
        /// <returns></returns>
        public static Vector2? LineToSegmentIntersectionPoint(
            Vector2 lineStart, Vector2 lineEnd, Vector2 segStart, Vector2 segEnd)
        {
            return LineToSegmentIntersectionPoint(
                lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y, segStart.X, segStart.Y, segEnd.X, segEnd.Y);
        }

        /// <summary>
        /// Calculate the intersection point between two line segments (or null if they
        /// do not intersect).
        /// </summary>
        public static Vector2? SegmentIntersectionPoint(
            double x1, double y1, double x2, double y2,
            double x3, double y3, double x4, double y4)
        {
            var p = LineIntersectionPoint(x1, y1, x2, y2, x3, y3, x4, y4);
            if (!p.HasValue)
                return null;
            var v = p.Value;
            if (IsPointInSegmentRange(v.X, v.Y, x1, y1, x2, y2) &&
                IsPointInSegmentRange(v.X, v.Y, x3, y3, x4, y4))
                return p;
            return null;
        }

        /// <summary>
        /// Calculate the intersection point between two line segments (or null if they
        /// do not intersect).
        /// </summary>
        /// <param name="start1"></param>
        /// <param name="end1"></param>
        /// <param name="start2"></param>
        /// <param name="end2"></param>
        /// <returns></returns>
        public static Vector2? SegmentIntersectionPoint(
            Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            return SegmentIntersectionPoint(start1.X, start1.Y, end1.X, end1.Y, start2.X, start2.Y, end2.X, end2.Y);
        }

        /// <summary>
        /// Check if a point lies on a given line.
        /// </summary>
        public static bool PointOnLine(
            double px, double py,
            double x1, double y1, double x2, double y2)
        {
            if (x1 == x2)
                return px == x1 && ((y1 <= py) == (py <= y2));
            else if (y1 == y2)
                return py == y1 && ((x1 <= px) == (px <= y1));
            double m = (y2 - y1) / (x2 - x1);
            double b = y1 - m * x1;
            return py == m * px + b;
        }

        /// <summary>
        /// Check if a point lies on a given line.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool PointOnLine(
            Vector2 point, Vector2 start, Vector2 end)
        {
            return PointOnLine(point.X, point.Y, start.X, start.Y, end.X, end.Y);
        }

        /// <summary>
        /// Check if a point lies on a given line segment.
        /// </summary>
        public static bool PointOnSegment(
            double px, double py,
            double x1, double y1, double x2, double y2)
        {
            return PointOnLine(px, py, x1, y1, x2, y2) && IsPointInSegmentRange(px, py, x1, y1, x2, y2);
        }

        /// <summary>
        /// Check if a point lies on a given line segment.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool PointOnSegment(
            Vector2 point, Vector2 start, Vector2 end)
        {
            return PointOnSegment(point.X, point.Y, start.X, start.Y, end.X, end.Y);
        }

    }
}
