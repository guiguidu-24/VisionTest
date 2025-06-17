using VisionTest.Core.Models;

namespace VisionTest.Core.Services.Storage
{
    internal class IndexationService(string enumFilePath, string projectDirectory)
    {
        private readonly string _enumFilePath = enumFilePath;
        private readonly string _projectDirectory = projectDirectory;

        internal async Task RemoveElementFromIndexAsync(string id)
        {
            if (File.Exists(_enumFilePath))
            {
                var lines = (await File.ReadAllLinesAsync(_enumFilePath)).ToList();
                lines.RemoveAll(line => line.Contains("public const " + id + " "));

                await File.WriteAllLinesAsync(_enumFilePath, lines);
            }
            else
            {
                throw new FileNotFoundException($"Enum file '{_enumFilePath}' does not exist.");
            }
        }

        internal async Task AddElementToIndexAsync(ScreenElement screenElement)
        {
            await Task.Run(async () =>
            {
                if (!File.Exists(_enumFilePath))
                {
                    using FileStream fileStream = File.Create(_enumFilePath);
                    using StreamWriter fileWriter = new(fileStream);
                    await fileWriter.WriteLineAsync($"namespace {Path.GetFileName(_projectDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))};");
                    await fileWriter.WriteAsync("public static class ScreenElements \n{\n}");
                }

                string enumContent = await File.ReadAllTextAsync(_enumFilePath);

                enumContent = enumContent.Insert(enumContent.LastIndexOf('}'), $"\tpublic const string {screenElement.Id} = \"{screenElement.Id}\";\n");

                using var writer = new StreamWriter(_enumFilePath, append: false);
                await writer.WriteAsync(enumContent);
            });
        }
    }
}
