using System.Drawing;
using VisionTest.Core.Models;
using VisionTest.Core.Services;

namespace VisionTest.Tests
{
    [TestFixture]
    public class ScreenElementStorageServiceTest
    {
        private IScreenElementStorageService _storageService;
        private readonly string _testDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestScriptData");

        [SetUp]
        public void Setup()
        {
            _storageService = new ScreenElementStorageService();
        }

        [Test]
        public async Task SaveAsync_image()
        {
            var element = new ScreenElement
            {
                Id = $"imageTest_{Guid.NewGuid()}",
            };
            element.Images.Add(new Bitmap(100, 100)); // Add a dummy image

            await _storageService.SaveAsync(element);
            Assert.IsTrue(File.Exists(Path.Combine(_testDirectory, $"{element.Id}.png")));


            File.Delete(Path.Combine(_testDirectory, $"{element.Id}.png"));
        }

        [Test]
        public async Task DeleteAsync_test()
        {
            var img = new Bitmap(100, 100);
            img.Save(Path.Combine(_testDirectory, "deleteTest.png"));
            Assert.IsTrue(File.Exists(Path.Combine(_testDirectory, "deleteTest.png")));

            await _storageService.DeleteAsync("deleteTest");

            Assert.IsFalse(File.Exists(Path.Combine(_testDirectory, "deleteTest.png")));
        }

        [Test]
        public async Task GetByIdAsync_test()
        {
            const string id = "Firefox";
            var element = await _storageService.GetByIdAsync(id);

            Assert.That(element, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(element.Id, Is.EqualTo(id));
                Assert.That(element.Images.Count, Is.EqualTo(1));
            });
        }


        [Test]
        public async Task GetAllAsync_test()
        {
            const string id = "Firefox";
            var elements = await _storageService.GetAllAsync();

            Assert.That(elements, Is.Not.Null);
            Assert.That(elements.Count(), Is.EqualTo(1));

            Assert.That(elements.First().Id, Is.EqualTo(id));
            Assert.That(elements.First().Images.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task ExistsAsync_test_true()
        {
            const string id = "Firefox";
            var exists = await _storageService.ExistsAsync(id);
            Assert.That(exists, Is.True);
        }

        [Test]
        public async Task ExistsAsync_test_false()
        {
            string id = "Firefox" + Guid.NewGuid();
            var exists = await _storageService.ExistsAsync(id);
            Assert.That(exists, Is.False);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up test directory after each test
            if (Directory.Exists(_testDirectory))
            {
                foreach (var file in Directory.GetFiles(_testDirectory))
                {
                    if (Path.GetFileNameWithoutExtension(file) != "Firefox") // Keep the Firefox test file
                        File.Delete(file);
                }
            }
        }
    }
}
