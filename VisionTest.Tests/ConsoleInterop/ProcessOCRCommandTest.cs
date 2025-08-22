using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Text;
using System.Text.Json;

namespace VisionTest.Tests.ConsoleInterop
{
    internal class ProcessOCRCommandTest
    {
        [Test]
        public void ProcessOCRCommand_PrintsExpectedJson_ForCottonLikeImage()
        {
            // Arrange
            var imagePath = @"C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.Tests\images\cottonLike.png";

            // Redirect console output
            var output = new StringBuilder();
            using var writer = new StringWriter(output);
            var originalOut = Console.Out;
            Console.SetOut(writer);

            try
            {
                // Act
                VisionTest.ConsoleInterop.Program.ProcessOCRCommand(imagePath);
                writer.Flush();
                var jsonOutput = output.ToString().Trim();

                // Parse JSON once
                using var doc = JsonDocument.Parse(jsonOutput);
                var root = doc.RootElement;
                root.TryGetProperty("response", out var response);
                response.TryGetProperty("status", out var status);
                response.TryGetProperty("message", out var message);
                response.TryGetProperty("data", out var data);
                data.TryGetProperty("textFound", out var textFound);

                // Assert all together
                Assert.Multiple(() =>
                {
                    Assert.That(root.TryGetProperty("response", out _), Is.True, "Missing 'response' property");
                    Assert.That(response.ValueKind, Is.EqualTo(JsonValueKind.Object), "‘response’ is not an object");

                    Assert.That(response.TryGetProperty("status", out _), Is.True, "Missing 'status' property");
                    Assert.That(status.GetString(), Is.EqualTo("success"), "Status should be 'success'");

                    Assert.That(response.TryGetProperty("message", out _), Is.True, "Missing 'message' property");
                    Assert.That(message.GetString(), Is.EqualTo("All text read"), "Message should be 'All text read'");

                    Assert.That(response.TryGetProperty("data", out _), Is.True, "Missing 'data' property");
                    Assert.That(data.ValueKind, Is.EqualTo(JsonValueKind.Object), "‘data’ is not an object");

                    Assert.That(data.TryGetProperty("textFound", out _), Is.True, "Missing 'textFound' property");
                    Assert.That(textFound.GetString(), Is.EqualTo("cotton-like"), "Extracted text should be 'cotton-like'");
                });
            }
            finally
            {
                // Restore console output
                Console.SetOut(originalOut);
            }
        }

        [Test]
        public void ProcessOCRCommand_PrintsErrorJson_WhenArgumentsAreMissing()
        {
            // Arrange
            var output = new StringBuilder();
            using var writer = new StringWriter(output);
            var originalOut = Console.Out;
            Console.SetOut(writer);

            try
            {
                // Act
                VisionTest.ConsoleInterop.Program.ProcessOCRCommand("");
                writer.Flush();
                var jsonOutput = output.ToString().Trim();

                // Parse JSON
                using var doc = JsonDocument.Parse(jsonOutput);
                var root = doc.RootElement;
                root.TryGetProperty("response", out var response);
                response.TryGetProperty("status", out var status);
                response.TryGetProperty("message", out var message);

                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(status.GetString(), Is.EqualTo("error"), "Status should be 'error'");
                    Assert.That(message.GetString(), Is.EqualTo("Image path cannot be empty."), "Message should indicate argument count error");
                    Assert.That(response.TryGetProperty("data", out _), Is.False, "Error response should not contain 'data' property");
                });
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Test]
        public void ProcessOCRCommand_PrintsErrorJson_WhenFileDoesNotExist()
        {
            // Arrange
            var imagePath = @"C:\nonexistent\file.png";

            var output = new StringBuilder();
            using var writer = new StringWriter(output);
            var originalOut = Console.Out;
            Console.SetOut(writer);

            try
            {
                // Act
                VisionTest.ConsoleInterop.Program.ProcessOCRCommand(imagePath);
                writer.Flush();
                var jsonOutput = output.ToString().Trim();

                // Parse JSON
                using var doc = JsonDocument.Parse(jsonOutput);
                var root = doc.RootElement;
                root.TryGetProperty("response", out var response);
                response.TryGetProperty("status", out var status);
                response.TryGetProperty("message", out var message);

                // Assert
                Assert.Multiple(() =>
                {
                    Assert.That(status.GetString(), Is.EqualTo("error"), "Status should be 'error'");
                    Assert.That(message.GetString(), Is.EqualTo($"The file {imagePath} does not exist."), "Message should indicate missing file");
                    Assert.That(response.TryGetProperty("data", out _), Is.False, "Error response should not contain 'data' property");
                });
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
    }
}
