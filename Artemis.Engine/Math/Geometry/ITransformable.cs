#region License

// Copyright (c) 2015 FCDM
// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is furnished 
// to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region Header

/* Author: Michael Ala
 * Date of Creation: 6/24/2015
 * 
 * Description
 * ===========
 *  An interface for objects that can be transformed geometrically.
 */

#endregion

#region Using Statements

using Microsoft.Xna.Framework;

#endregion

namespace Phosphaze.Framework.Maths.Geometry
{
    public interface ITransformable
    {

        void SetPosition(double x, double y);

        void SetPosition(Vector2 pos);

        void SetPositionX(double x);

        void SetPositionY(double y);

        void Translate(double dx, double dy);

        void Translate(Vector2 delta);

        void Rotate(double angle, bool degrees = true);

        void Rotate(double angle, Vector2 origin, bool degrees = true, bool absoluteOrigin = true);

        void Scale(double amount);

        void Scale(double amount, Vector2 origin, bool absoluteOrigin = true);

    }
}
