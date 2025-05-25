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
            testExecutor.Click(new Bitmap("C:\\Users\\guill\\Programmation\\dotNET_doc\\VisionTest\\VisionTest.TestsImplementation\\TestScriptData\\Firefox.png"));

            testExecutor.Click("Rechercher", "C:\\Users\\guill\\Programmation\\dotNET_doc\\VisionTest\\VisionTest.TestsImplementation\\TestScriptData\\rechercher_ytb.png");

            keyboard.TypeText("blueg");

            testExecutor.Click("bluegrass");
        }
    }
}
