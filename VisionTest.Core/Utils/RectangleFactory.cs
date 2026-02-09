using System;
using System.Collections.Generic;
using System.Text;

namespace VisionTest.Core.Utils
{
    public static class RectangleFactory
    {
        public static Rectangle FromPoints(Point upperLeft, Point lowerRight)
        {
            return new Rectangle(upperLeft.X, upperLeft.Y, lowerRight.X - upperLeft.X, lowerRight.Y - upperLeft.Y);
        }
    }
}
