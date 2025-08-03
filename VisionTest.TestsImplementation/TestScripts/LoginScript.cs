using NUnit.Framework;
using System;
using System.Drawing;
using System.Threading.Tasks;
using VisionTest.Core.Services;
using VisionTest.Core.Services.Storage;

namespace VisionTest.TestsImplementation
{
    internal class LoginScript //TODO Implement a password secured insertion method
    {
        private TestExecutor tester;
        private RepositoryManager repositoryManager = new RepositoryManager(@"C: \Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.TestsImplementation\");

        [SetUp]
        public void Setup()
        {
            tester = new TestExecutor();
        }

        [Test]
        public async Task Login()
        {
            tester.Click(await repositoryManager.GetByIdAsync(ScreenElements.star) ?? throw new Exception("element does not exist"));
        }
    }
}
