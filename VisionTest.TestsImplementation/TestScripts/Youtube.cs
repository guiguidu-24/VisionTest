using System.Drawing;
using VisionTest.Core;
using VisionTest.Core.Input;
using VisionTest.Core.Recognition;
using VisionTest.Core.Utils;

namespace VisionTest.TestsImplementation.TestScripts
{
    [TestFixture]
    internal class Youtube
    {
        [Test]
        public async Task Run()
        {
            using var ffx = new Bitmap("C:\\Users\\guill\\Programmation\\dotNET_doc\\VisionTest\\VisionTest.TestsImplementation\\TestScriptData\\Firefox.png");
            var firefox = new LocatorV(ffx);
            var lettersOnly = new OcrOptions(whiteListChar: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ", lang: Language.French);
            var barreDeRecherche = new LocatorV("Rechercher", lettersOnly);
            var kb = new Keyboard();
            var title = new LocatorV("Greatest Bluegrass", lettersOnly);




            await firefox.ClickAsync();

            ScreenElement barreDeRechercheArea = (await barreDeRecherche.WaitForAsync()).Click();
            barreDeRechercheArea.Click();
            

            kb.TypeText("bluegrass");
            kb.PressKey(KeyCode.Enter);

            await title.HoverAsync();

            await Task.Delay(2000);

            barreDeRechercheArea.DoubleClick();

            var bluegrassMusicArea = await (new LocatorV("bluegrass music")).WaitForAsync();

            var interestArea = RectangleFactory.FromPoints(barreDeRechercheArea.Bounds.LowerLeft(), bluegrassMusicArea.Bounds.UpperRight());
            var bg = new LocatorV("bluegrass", lettersOnly, interestArea);

            var rect = await bg.WaitForAsync();

            rect.Hover();
        }
    }
}
