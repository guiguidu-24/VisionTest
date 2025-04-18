using OpenCvSharp;
using POC_Tesseract;

var appli = new Appli("notepad", new string[] { "E:\\Projects data\\POC_Tesseract\\TestTesseract\\engText.txt" });
var imgEngine = new ImgEngine(0.9f);

var img = appli.GetScreen();
var target = new Bitmap("E:\\Projects data\\POC_Tesseract\\TestTesseract\\cotton-like.png");

if (imgEngine.Find(img, target, out Rectangle area))
{
    Pen pen = new Pen(Color.Red, 2);
    using (Graphics g = Graphics.FromImage(img))
    {
        g.DrawRectangle(pen, area);
    }
    img.Save("E:\\Projects data\\POC_Tesseract\\TestTesseract\\cottonResult.png");
}
else
{
    Console.WriteLine("No match found.");
}

