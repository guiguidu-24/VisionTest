using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionTest.Core.Input;
using VisionTest.Core.Services;

namespace VisionTest.TestsImplementation.TestScripts
{
    [TestFixture]
    internal class Youtube
    {
        private readonly TestExecutor testExecutor = new TestExecutor();
        private readonly IKeyboard keyboard = new Keyboard();

        [Test]
        public void Run()
        {
            //testExecutor.Open();
            testExecutor.Click(new Bitmap("C:\\Users\\guill\\Programmation\\dotNET_doc\\VisionTest\\TestsImplementation\\TestScriptData\\Firefox.png"));

            testExecutor.Click("Rechercher", "C:\\Users\\guill\\Programmation\\dotNET_doc\\VisionTest\\TestsImplementation\\TestScriptData\\Rechercher.png", 10000); //TODO not working

            keyboard.TypeText("blueg");

            testExecutor.Click("bluegrass");

            testExecutor.Wait(10000);

        }


        [SetUp]
        public void Setup()
        {
            //testExecutor.AppPath = LocalResources.Firefox;
            //testExecutor.Arguments = [ "www.youtube.com" ];
        }


        [TearDown]
        public void TearDown()
        {
            //testExecutor.Close();
        }
    }
}
