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


//TODO : Make an element recorder for the tests. The recorder should be a Visual Studio Extension with a window
//that pops up and enables you to make image treatment and choose witch attributes you want to use.