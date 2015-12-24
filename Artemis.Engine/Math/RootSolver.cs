#region Using Statements

using System;
using System.Linq;

#endregion

namespace Artemis.Engine.Math
{
    public static class RootSolver
    {

        /// <summary>
        /// Solve for the real roots of a quadratic. (Ax^2 + Bx + C == 0)
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public static double[] Quadratic(double A, double B, double C)
        {
            if (A == 0)
            {
                if (B == 0)
                    throw new ArgumentException(
                        "Invalid quadratic. The first and second " +
                        "coefficients cannot both be zero."
                        );
                return new double[] { -C / B };
            }
            double r = B * B - 4 * A * C;
            if (System.Math.Abs(r) < 1e-8)
                return new double[] { -B / (2 * A) };
            else if (r < 0)
                return new double[] { };
            r = System.Math.Sqrt(r);
            double A2 = 2 * A;
            return new double[] {
                ((-B + r)/A2),
                ((-B - r)/A2)
            };
        }

        /// <summary>
        /// Solve for the real roots of a cubic. (Ax^3 + Bx^2 + Cx + D == 0)
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <returns></returns>
        public static double[] Cubic(double A, double B, double C, double D)
        {
            if (A == 0)
                return Quadratic(B, C, D);
            double B_over_A = B / A;
            double F, G, H;
            
            F = ((3*C / A) - (System.Math.Pow(B_over_A, 2.0))) / 3;
            G = ((2 * System.Math.Pow(B_over_A, 3.0)) - (9 * C * B_over_A / A) + (27 * D / A)) / 27;
            double G_over_2 = -G / 2.0;
            double G_over_2_sqrd = G_over_2 * G_over_2;
            H = G_over_2_sqrd + (System.Math.Pow(F / 3.0, 3.0));

            // Only one root is real.
            if (H > 0)
            {
                double sqrt_H = System.Math.Sqrt(H);
                double R, S, T, U;
                
                R = G_over_2 + sqrt_H;
                if (R < 0)
                    S = -System.Math.Pow(-R, 1.0 / 3.0);
                else
                    S = System.Math.Pow(R, 1.0 / 3.0);

                T = G_over_2 - sqrt_H;
                if (T < 0)
                    U = -System.Math.Pow(-T, 1.0 / 3.0);
                else
                    U = System.Math.Pow(T, 1.0 / 3.0);

                double X = S + U - B_over_A / 3.0;
                return new double[] { X };
            }
            // All 3 roots are real and equal.
            else if (F == G && G == H && H == 0)
            {
                double D_A = D / A;
                double X;
                if (D_A < 0)
                    X = System.Math.Pow(-D_A, 1.0 / 3.0);
                else
                    X = -System.Math.Pow(D_A, 1.0 / 3.0);
                return new double[] { X };
            }

            // All 3 roots are real.
            double I, J, K, L, M, N, P;
            I = System.Math.Sqrt(G_over_2_sqrd - H);
            J = System.Math.Pow(I, 1.0 / 3.0);
            K = System.Math.Acos(-(G / (2 * I))) / 3.0;
            L = -J;
            M = System.Math.Cos(K);
            N = System.Math.Sqrt(3) * System.Math.Sin(K);
            P = -B_over_A / 3.0;

            double X1, X2, X3;
            X1 = 2*J*System.Math.Cos(K) + P;
            X2 = L*(M + N) + P;
            X3 = L*(M - N) + P;

            var result = new double[] { X1, X2, X3 };
            Array.Sort(result);
            return result;
        }

        /// <summary>
        /// Use the Newton-Raphson method to attempt to approximate the root of a function.
        /// 
        /// In particular, this function takes in three parameters, f, df, and v. f and df
        /// are the function and its derivative respectively. This method then returns
        /// x such that f(x) = v (i.e. the root of f(x) - v = 0).
        /// </summary>
        /// <param name="f"></param>
        /// <param name="df"></param>
        /// <param name="value"></param>
        /// <param name="initialGuess"></param>
        /// <param name="epsilon"></param>
        /// <param name="maxIterations"></param>
        /// <returns></returns>
        public static double NewtonsMethod(
            Func<double, double> f, 
            Func<double, double> df, 
            double value, 
            double initialGuess = 0,
            double epsilon = 1e-9,
            int maxIterations=1000)
        {
            double x_prev = 0;
            double xn = initialGuess;

            int currentIteration = 0;
            var maxIterationsReached = new NewtonsMethodException(
                "Max iterations reached; root does not converge.");

            do
            {
                if (currentIteration == maxIterations)
                    throw maxIterationsReached;
                x_prev = xn;
                xn = x_prev - (f(x_prev) - value) / df(x_prev);
                currentIteration++;
            } while (System.Math.Abs(xn - x_prev) > epsilon);

            return xn;
        }
        
    }
}
