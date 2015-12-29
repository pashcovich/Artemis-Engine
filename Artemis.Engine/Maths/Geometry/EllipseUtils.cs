#region Using Statements

using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;

#endregion

namespace Artemis.Engine.Maths.Geometry
{
    public static class EllipseUtils
    {

        /// <summary>
        /// Test if an ellipse overlaps a given line segment.
        /// </summary>
        public static bool EllipseOverlapSegment(
            Vector2 K, double A, double B,
            Vector2 P0, Vector2 P1)
        {
            // For the math, see:
            //   http://www.geometrictools.com/Documentation/IntersectionRectangleEllipse.pdf
            double q0, q1, q2;

            var Q0 = new Vector2((float)((P0.X - K.X) / A), (float)((P0.Y - K.Y) / B));
            var Q1 = new Vector2((float)((P1.X - P0.X) / A), (float)((P1.X - P0.X) / B));
            
            q0 = Vector2.Dot(Q0, Q0) - 1;
            q1 = Vector2.Dot(Q0, Q1);
            q2 = Vector2.Dot(Q1, Q1);

            if (q2 == 0)
            {
                double d = -q0 / q1;
                return 0 <= d && d <= 1;
            }

            double discr = q1 * q1 - q0 * q2;
            if (discr < 0)
                return false;

            double sqrt_discr = System.Math.Sqrt(discr);
            return (sqrt_discr - q1) >= 0 && (sqrt_discr + q1) >= -q2;
        }

        /// <summary>
        /// Find the intersection points between an ellipse and a given line.
        /// </summary>
        public static Vector2[] EllipseLineIntersections(
            double H, double K, double A, double B,
            double X1, double Y1, double X2, double Y2)
        {
            // This is pretty inefficient tbh, look for a better method.

            // If it's vertical, we can't calculate it's slope.
            if (X1 == X2)
            {
                if (System.Math.Abs(X1 - H) > A)
                    return new Vector2[] { };
                double V = B * System.Math.Sqrt(1 - System.Math.Pow((X1 - H) / A, 2.0));
                double iy1 = K + V;
                double iy2 = K - V;
                float fX1 = (float)X1;
                return new Vector2[] {
                    new Vector2(fX1, (float)(iy1)),
                    new Vector2(fX1, (float)(iy2))
                };
            }
            else
            {
                var intersections = new List<Vector2>();
                double m, c, a_sqrd, b_sqrd, U, V, W, discr;

                a_sqrd = A * A;
                b_sqrd = B * B;

                m = (Y2 - Y1) / (X2 - X1);
                c = Y2 - m * X2;


                U = a_sqrd * m * m + b_sqrd;
                V = 2 * (a_sqrd * m * (c - K) - b_sqrd);
                W = a_sqrd * System.Math.Pow(c - K, 2.0) - a_sqrd * b_sqrd + b_sqrd * H * H;

                discr = V * V - 4 * U * W;
                if (discr < 0)
                    return intersections.ToArray();
                else if (discr == 0)
                {
                    double ix = -V / (2 * U);
                    double iy = m * ix + c;
                    intersections.Add(new Vector2((float)ix, (float)iy));
                }
                else
                {
                    double U2 = 2 * U;
                    double sqrt_discr = System.Math.Sqrt(discr);
                    double ix1 = (sqrt_discr - V) / U2;
                    double ix2 = (-sqrt_discr - V) / U2;
                    intersections.Add(new Vector2((float)ix1, (float)(m * ix1 + c)));
                    intersections.Add(new Vector2((float)ix2, (float)(m * ix2 + c)));
                }
                return intersections.ToArray();
            }
        }

        // Not finished the math yet. Also, there's probably a better solution. Something
        // involving Lagrange multipliers to minimize the multivariate conic equations in
        // standard form or something.

        /*
        public static Vector2[] EllipseEllipseIntersection(
            double h1, double k1, double a1, double b1,
            double h2, double k2, double a2, double b2)
        {
            // This better fucking work.

            double a1_sqrd = a1 * a1, b1_sqrd = b1 * b1;
            double a2_sqrd = a2 * a2, b2_sqrd = b2 * b2;

            // The entries of the two ellipses in general conic matrix form:
            //      [A, 0, D]
            //      [0, C, E]
            //      [D, E, F]
            // We will denote the matrices for each ellipse L1 and L2 respectively.
            // Note that the zeros are B-Terms, which don't exist because both ellipses
            // are oriented such that their axes are orthogonal to the coordinate space.
            double A1, C1, D1, E1, F1;
            double A2, C2, D2, E2, F2;

            A1 = 1 / a1_sqrd;
            C1 = 1 / b1_sqrd;
            D1 = -h1 / a1_sqrd;
            E1 = -k1 / b1_sqrd;
            F1 = h1 * h1 / a1_sqrd + k1 * k1 / b1_sqrd - 1;

            A2 = 1 / a2_sqrd;
            C2 = 1 / b2_sqrd;
            D2 = -h2 / a2_sqrd;
            E2 = -k2 / b2_sqrd;
            F2 = h2 * h2 / a2_sqrd + k2 * k2 / b2_sqrd - 1;

            double N1, N2, N3, N4, N5, N6;
            N1 = C2 * F1 - E1 * E2;
            N2 = C2 * F2 - E2 * E2;
            N3 = A1 * D2 - A2 * D1;
            N4 = C1 * E2 - E1 * C2;
            N5 = A2 * F2 - D2 * D2;
            N6 = C2 * D1 * D2;

            // The 9 entries of the matrix A.
            // A is defined as L1 * adj(L2) where adj(M) represents the adjugate matrix of M.
            // For any invertible matrix M, M^-1 = 1/det(M)*adj(M).
            double M11, M12, M13, M21, M22, M23, M31, M32, M33;
            M11 = A1 * N2 - N6;
            M12 = E2 * N3;
            M13 = C2 * (-N3);
            M21 = D2 * N4;
            M22 = C1 * N5 - A2 * E1 * E2;
            M23 = A2 * (-N4);
            M31 = D1 * N2 - D2 * N1;
            M32 = E1 * N5 + E2 * (D1 * D2 - A2 * F1);
            M33 = A2 * N1 - N6;

            // The reciprocal of the determinant of the second ellipse.
            double sigma = A2 * N2 - C2 * D2 * D2;

            // Coefficients of the characteristic equation of A.
            // Px^3 + Qx^2 + Rx + S = 0
            double P, Q, R, S;
            P = -Math.Pow(sigma, 3.0);
            Q = sigma * sigma * (M11 + M22 + M33);
            R = 2 * sigma * (M11 * M22 + M11 * M33 + M22 * M33 - M12 * M21 - M13 * M31 - M23 * M32);
            S = M11 * (M22 * M33 - M23 * M32) - M12 * (M21 * M33 - M23 * M31) + M13 * (M21 * M32 - M22 * M31);

            double lambda = RootSolver.Cubic(P, Q, R, S)[0]; // Guaranteed to always have one root.
        }
         */
    }
}
