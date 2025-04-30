using NUnit.Framework;
using POC_Tesseract;
using System.Drawing;

namespace TestsImplementation
{
    internal class LoginScript //TODO Implement a password secured insertion method
    {
        private Appli appli;

        [SetUp]
        public void Setup()
        {
            appli = new Appli(@"C:\Program Files(x86)\Microsoft\Edge\Application\msedge.exe", ["https://ds006g7k:9443/jts/dashboards"]);

        }

        [Test]
        public void Login()
        {
           
        }
    }
}
