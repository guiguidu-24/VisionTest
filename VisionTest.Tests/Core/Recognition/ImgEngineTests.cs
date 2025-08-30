using System.Drawing;
using System.Reflection;
using VisionTest.Core.Recognition;

namespace VisionTest.Tests.Core.Recognition;

[TestFixture]
public class ImgEngineTests
{
    [Test]
    public async Task Find_ShouldReturnCorrectMatches()
    {
        // Arrange
        var options = new ImgOptions();
        var engine = new ImgEngine(options);

        var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("The assembly path is null");
        using var sourceImage = new Bitmap(Path.Combine(assemblyDir, "images/big.png"));
        using var targetImage = new Bitmap(Path.Combine(assemblyDir, "images/small.png"));

        // Act
        var matches = engine.Find(sourceImage, targetImage).ToList();

        // Assert
        await Verify(matches);
    }
}
