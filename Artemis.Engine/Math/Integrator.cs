#region Using Statements

using System;

#endregion

namespace Artemis.Engine.Math
{
    public static class Integrator
    {

        // Note to self: Never use Romberg's method in the future, it is utter garbage.

        /// <summary>
        /// Integrate the given function <code>f</code> between the given limits <code>a</code>
        /// and <code>b</code> using Simpson's method with <code>n</code> steps.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double Simpsons(Func<double, double> f, double a, double b, int n)
        {
            if (n % 2 != 0)
                throw new ArgumentException(
                    String.Format(
                        "For Simpson's method to be used, the number of steps must be even. " +
                        "The number of steps received was {0}.", n
                        )
                    );
            if (a == b)
            {
                return 0;
            }
            double s = (b - a) / n, alpha = s / 3.0, interval = s, m = 4.0;
            double sum = f(a) + f(b);

            for (int i = 0; i < n - 1; i++)
            {
                sum += m * f(a + interval);
                m = 6 - m;
                interval += s;
            }
            return alpha * sum;
        }

    }
}