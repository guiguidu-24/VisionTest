using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using NUnit.Framework;

namespace POC_Tesseract.TestScripts
{
    internal class LoginScript
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
