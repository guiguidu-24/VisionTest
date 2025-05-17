namespace Core.Models
{
    public class ScreenElement //TODO : an element can have multiple images of reference and image treatment settings for each one
    {
        public IEnumerable<Rectangle> Boxes { get; } = new List<Rectangle>();
        public IEnumerable<Bitmap> Images { get; } = new List<Bitmap>();
        public IEnumerable<string> Texts { get; } = new List<string>();
        //TODO add Selenium capabilities
    }
}
