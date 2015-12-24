
namespace Artemis.Engine.Math.Geometry
{
    public static class RectUtils
    {

        /// <summary>
        /// Check if two rectangles are overlapping.
        /// </summary>
        public static bool Collision(
            double x1, double y1, double w1, double h1,
            double x2, double y2, double w2, double h2)
        {
            return x1 <= x2 + w2 &&
                   x1 + w1 >= x2 &&
                   y1 <= y2 + h2 &&
                   y1 + h1 >= y2;
        }

        /// <summary>
        /// Check if the first rectangle entirely encloses the second rectangle.
        /// </summary>
        public static bool Contains(
            double x1, double y1, double w1, double h1,
            double x2, double y2, double w2, double h2)
        {
            return x1 <= x2 && x1 + w1 >= x2 + w2 &&
                   y1 <= y2 && y1 + h1 >= y2 + h2;
        }

        /// <summary>
        /// Check if the two given rectangles form a cross pattern.
        /// </summary>
        public static bool Crosses(
            double x1, double y1, double w1, double h1,
            double x2, double y2, double w2, double h2)
        {
            return (
                    x1 <= x2 && x1 + w1 >= x2 + w2 &&
                    y1 >= y2 && y1 + h1 <= y2 + h2
                ) || (
                    x1 >= x2 && x1 + w1 <= x2 + w2 &&
                    y1 <= y2 && y1 + h1 >= y2 + h2
                );
        }

    }
}
