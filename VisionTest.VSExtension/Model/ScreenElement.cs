
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace VisionTest.VSExtension
{
    public class ScreenElement 
    {
        //public List<Rectangle> Boxes { get; private set; } = new List<Rectangle>(); //TODO : add boxes
        //public List<BitmapImage> Images { get; private set; } = new List<BitmapImage>();
        //public List<By> Locators { get; private set; } = new List<By>();      // TODO : add locators
        //public List<string> Texts { get; private set; } = new List<string>(); 
        public BitmapImage Image { get; set; }
        public string Text { get; set; }
        public string Id { get; set; }

    }
}
