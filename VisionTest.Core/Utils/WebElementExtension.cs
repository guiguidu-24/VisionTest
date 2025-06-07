using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionTest.Core.Utils
{
    public static class WebElementExtension
    {
        private static Rectangle ToRectangle(this IWebElement webElement)
        {
            return new Rectangle(webElement.Location,webElement.Size);
        }

        public static bool Inside(this IWebElement webElement, Rectangle rectangle2)
        {
            var rectangle1 = webElement.ToRectangle();

            return rectangle1.X >= rectangle2.X &&
        rectangle1.Y >= rectangle2.Y &&
        rectangle1.Right <= rectangle2.Right &&
        rectangle1.Bottom <= rectangle2.Bottom;
        }
    }
}
