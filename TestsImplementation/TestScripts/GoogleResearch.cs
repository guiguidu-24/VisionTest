using Core.Input;
using Core.Services;
using Core.Utils;
using System.Drawing;
using WindowsInput.Events;

namespace TestsImplementation.TestScripts
{
    [TestFixture]
    internal class GoogleResearch
    {
        private TestExecutor appli;
        private IKeyboard keyboard;

        [Test]
        public void Execute()
        {
            Point point = appli.WaitFor(new Bitmap(LocalResources.TestScriptData + @"\searchbar_google.png")).Center();
            Console.WriteLine($"Found at: {point.X}, {point.Y}");
            var screenshot = new Screen().CaptureScreen();
            screenshot.Save("C:\\Users\\guill\\Programmation\\dotNET_doc\\POC_Tesseract\\TestsImplementation\\screenshot.png");
            appli.Click(point);
            keyboard.TypeText("Tesseract");
            keyboard.PressKey(KeyCode.Enter);
            appli.WaitFor("Wikipédia");
        }

        [SetUp]
        public void Setup()
        {
            keyboard = new Keyboard();
            appli = new TestExecutor();
            appli.AppPath = LocalResources.Msedge;
            appli.Arguments = ["www.google.com"];
            appli.Open();
            appli.Wait(1000);
        }

        [TearDown]
        public void TearDown()
        {
            appli.Close();
        }
    }
}
