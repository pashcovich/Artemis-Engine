#region Using Statements

using System;

#endregion

namespace Artemis.Engine.Maths
{
    /// <summary>
    /// A solver for an ODE of the form F(x, y) = y'. This uses the standard Runge-Kutta
    /// method.
    /// </summary>
    public class ODESolver
    {

        /// <summary>
        /// The function F in the ODE F(x, y) = y'. 
        /// </summary>
        public Func<double, double, double> Function { get; private set; }

        /// <summary>
        /// The amount by which to increment x each time GetNext() is called.
        /// </summary>
        public double stepSize { get; private set; }

        /// <summary>
        /// The initial X value.
        /// </summary>
        public double initialX { get; set; }

        /// <summary>
        /// The initial Y value.
        /// </summary>
        public double initialY { get; set; }

        /// <summary>
        /// The current X value.
        /// </summary>
        public double currentX { get; private set; }

        /// <summary>
        /// The current Y value (what the ODESolver solves for each time GetNext is called).
        /// </summary>
        public double currentY { get; private set; }

        public ODESolver(Func<double, double, double> f, double stepSize, double initialX, double initialY)
        {
            Function = f;
            this.stepSize = stepSize;
            this.initialX = initialX;
            this.initialY = initialY;
            this.currentX = initialX;
            this.currentY = initialY;
        }

        /// <summary>
        /// Calculate the next Y value in the approximation.
        /// </summary>
        /// <returns></returns>
        public double GetNext()
        {
            double k1, k2, k3, k4;
            double h_2 = stepSize / 2;
            currentX += stepSize;
            k1 = Function(currentX, currentY);
            k2 = Function(currentX + h_2, currentY + h_2 * k1);
            k3 = Function(currentX + h_2, currentY + h_2 * k2);
            k4 = Function(currentX + stepSize, currentY + stepSize * k3);
            currentY += stepSize / 6 * (k1 + 2 * (k2 + k3) + k4);
            return currentY;
        }
    }
}
