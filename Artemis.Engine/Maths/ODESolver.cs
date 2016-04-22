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
        public double StepSize { get; private set; }

        /// <summary>
        /// The initial X value.
        /// </summary>
        public double InitialX { get; set; }

        /// <summary>
        /// The initial Y value.
        /// </summary>
        public double InitialY { get; set; }

        /// <summary>
        /// The current X value.
        /// </summary>
        public double CurrentX { get; private set; }

        /// <summary>
        /// The current Y value (what the ODESolver solves for each time GetNext is called).
        /// </summary>
        public double CurrentY { get; private set; }

        public ODESolver(Func<double, double, double> f, double stepSize, double initialX, double initialY)
        {
            Function = f;
            this.StepSize = stepSize;
            this.InitialX = initialX;
            this.InitialY = initialY;
            this.CurrentX = initialX;
            this.CurrentY = initialY;
        }

        /// <summary>
        /// Calculate the next Y value in the approximation.
        /// </summary>
        /// <returns></returns>
        public double GetNext()
        {
            double k1, k2, k3, k4;
            double h_2 = StepSize / 2;
            CurrentX += StepSize;
            k1 = Function(CurrentX, CurrentY);
            k2 = Function(CurrentX + h_2, CurrentY + h_2 * k1);
            k3 = Function(CurrentX + h_2, CurrentY + h_2 * k2);
            k4 = Function(CurrentX + StepSize, CurrentY + StepSize * k3);
            CurrentY += StepSize / 6 * (k1 + 2 * (k2 + k3) + k4);
            return CurrentY;
        }
    }
}
