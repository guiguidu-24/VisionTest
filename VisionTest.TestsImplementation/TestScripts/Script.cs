using NUnit.Framework;

namespace POC_Tesseract.TestScripts
{
    [TestFixture]
    internal abstract class Script
    {

        public abstract void Setup();

        public abstract void Execute();

        public abstract void TearDown();

    }
}
