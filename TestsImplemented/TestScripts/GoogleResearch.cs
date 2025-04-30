using NUnit.Framework;
using POC_Tesseract;

namespace TestsImplemented.TestScripts
{
    [TestFixture]
    internal class GoogleResearch
    {
        private Appli appli;

        [Test]
        public void Execute()
        {
            appli.WaitFor(new Bitmap( LocalResources.TestScriptData + @"\oo_googleResearch.png"));
            appli.Click(new Bitmap(LocalResources.TestScriptData + @"\searchbar_google.png"));
            appli.Write("Tesseract");
            appli.Click("Recherche Google");
            appli.WaitFor("Wikipedia");
        }

        [SetUp]
        public void Setup()
        {
            appli = new Appli(LocalResources.Msedge, ["www.google.com"]);
            appli.Open();
            appli.Wait(2000);
        }

        [TearDown]
        public void TearDown()
        {
            appli.CloseWindow();
        }
    }
}
