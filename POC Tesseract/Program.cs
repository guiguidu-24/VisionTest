using WindowsInput;
using WindowsInput.Events;

namespace POC_Tesseract
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var appli = new Appli("notepad");
            appli.Open();
            appli.Wait(1000);
            var screen = appli.GetScreen();
            screen.Save("C:\\Users\\guill\\Programmation\\dotNET_doc\\POC_Tesseract\\POC Tesseract\\screenshot.png");
        }
    }
}