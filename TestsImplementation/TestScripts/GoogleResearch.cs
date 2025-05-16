using Core.Services;
using System.Drawing;
using WindowsInput;

namespace TestsImplementation
{
    [TestFixture]
    internal class GoogleResearch
    {
        private TestExecutor appli;

        [Test]
        public void Execute()
        {
            var point = appli.WaitFor(new Bitmap(LocalResources.TestScriptData + @"\searchbar_google.png"));
            Console.WriteLine($"Found at: {point.X}, {point.Y}");
            appli.Click(point);
            appli.Write("Tesseract");
            Simulate.Events().Click(WindowsInput.Events.KeyCode.Enter).Invoke().Wait();
            appli.WaitFor("Wikipédia");
        }

        [SetUp]
        public void Setup()
        {
            appli = new TestExecutor(LocalResources.Msedge, ["www.google.com"]);
            appli.Open();
            appli.Wait(1000);
        }

        [TearDown]
        public void TearDown()
        {
            appli.CloseWindow();
        }
    }
}
