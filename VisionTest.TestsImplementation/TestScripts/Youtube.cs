using System.Drawing;
using VisionTest.Core;
using VisionTest.Core.Input;
using VisionTest.Core.Recognition;

namespace VisionTest.TestsImplementation.TestScripts
{
    [TestFixture]
    internal class Youtube
    {
        [Test]
        public async Task Run()
        {
            var firefox = new LocatorV(new Bitmap("C:\\Users\\guill\\Programmation\\dotNET_doc\\VisionTest\\VisionTest.TestsImplementation\\TestScriptData\\Firefox.png"));
            await firefox.ClickAsync();

            var lettersOnly = new OcrOptions(whiteListChar: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ");
            var barreDeRecherche = new LocatorV("Rechercher", lettersOnly);

            await barreDeRecherche.ClickAsync();
            var kb = new Keyboard();

            kb.TypeText("bluegrass");

            var bluegrass = new LocatorV("bluegrass", lettersOnly);
            await bluegrass.ClickAsync();

            var title = new LocatorV("Greatest Bluegrass", lettersOnly);
            await title.ClickAsync();
        }
    }
}
