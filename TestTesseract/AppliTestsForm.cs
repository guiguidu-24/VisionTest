using POC_Tesseract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSUT;
using WindowsInput;

namespace TestTesseract
{
    internal class AppliTestsForm
    {
        private Appli appli;

        [SetUp]
        public void Setup()
        {
            // Initialisation avant chaque test
            appli = new Appli("cmd");
            TestSUT.Program.Main();
            Thread.Sleep(1000); // Wait a bit to ensure the process is started
        }

        [TearDown]
        public void TearDown()
        {
            appli.CloseWindow();
            TestSUT.Program.Close();
        }

        [Test]
        public void Write_ShouldSimulateTextInput()
        {
            Simulate.Events().Click(WindowsInput.Events.KeyCode.Tab).Invoke().Wait();
            appli.Write("Hello, world!");
            Assert.That(Form1.textBoxContent == "Hello, world!", Is.True, "Le texte écrit dans le TextBox devrait être 'Hello, world!'");
        }


    }
}
