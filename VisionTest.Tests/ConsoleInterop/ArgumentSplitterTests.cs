using VisionTest.ConsoleInterop;

namespace VisionTest.Tests.ConsoleInterop
{
    [TestFixture]
    public class ArgumentSplitterTests
    {
        [Test]
        public void SplitArguments_NoQuotes_ReturnsAllTokens()
        {
            // Arrange
            var input = "one two three";

            // Act
            var result = Program.SplitArguments(input);

            // Assert
            Assert.That(result.Length, Is.EqualTo(3));
            Assert.That(result, Is.EqualTo(new[] { "one", "two", "three" }));
        }

        [Test]
        public void SplitArguments_DoubleQuotedSegment_IsKeptIntact()
        {
            // Arrange
            var input = "cmd \"arg with spaces\" end";

            // Act
            var result = Program.SplitArguments(input);

            // Assert
            Assert.That(result.Length, Is.EqualTo(3));
            Assert.That(result[0], Is.EqualTo("cmd"));
            Assert.That(result[1], Is.EqualTo("arg with spaces"));
            Assert.That(result[2], Is.EqualTo("end"));
        }

        [Test]
        public void SplitArguments_SingleQuotedSegment_IsKeptIntact()
        {
            // Arrange
            var input = "cmd 'another test' tail";

            // Act
            var result = Program.SplitArguments(input);

            // Assert
            Assert.That(result.Length, Is.EqualTo(3));
            Assert.That(result[0], Is.EqualTo("cmd"));
            Assert.That(result[1], Is.EqualTo("another test"));
            Assert.That(result[2], Is.EqualTo("tail"));
        }

        [Test]
        public void SplitArguments_MixedQuotesAndWhitespace_AllTokensCorrect()
        {
            // Arrange
            var input = "  'a'   \"b c\"   d  ";

            // Act
            var result = Program.SplitArguments(input);

            // Assert
            Assert.That(result.Length, Is.EqualTo(3));
            Assert.That(result[0], Is.EqualTo("a"));
            Assert.That(result[1], Is.EqualTo("b c"));
            Assert.That(result[2], Is.EqualTo("d"));
        }
    }
}
