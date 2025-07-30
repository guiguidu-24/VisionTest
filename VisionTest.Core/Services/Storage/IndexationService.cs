using System.Globalization;
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
                    using var fileStream = File.Create(_enumFilePath);
                    using var fileWriter = new StreamWriter(fileStream);
                    await fileWriter.WriteLineAsync(
                        $"namespace {Path.GetFileName(_projectDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))};");
                    await fileWriter.WriteLineAsync("public static class ScreenElements");
                    await fileWriter.WriteLineAsync("{");
                    await fileWriter.WriteLineAsync("}");
                }

                var enumContent = await File.ReadAllTextAsync(_enumFilePath);

                // Split Id into path segments (e.g. "tem/debug1" → ["tem","debug1"])
                var parts = screenElement.Id.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                var className = parts.Length > 1 ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(parts[0]) : null;
                var constantName = parts.Last();

                // Prepare constant line
                var constLine = $"\t\tpublic const string {constantName} = \"{screenElement.Id.Replace('\\', '/')}\";";

                if (className is null)
                {
                    // Insert at root
                    var insertPos = enumContent.LastIndexOf('}');
                    enumContent = enumContent.Insert(insertPos, "\t" + constLine + "\n");
                }
                else
                {
                    // Ensure nested class exists
                    var nestedClassPattern = $"public static class {className}";
                    if (!enumContent.Contains(nestedClassPattern))
                    {
                        // Insert the nested class before the last '}' of ScreenElements
                        var rootClose = enumContent.LastIndexOf('}');
                        var nestedClassDef =
                            $"\tpublic static class {className}\n" +
                            "\t{\n" +
                            "\t}\n\n";
                        enumContent = enumContent.Insert(rootClose, nestedClassDef);
                    }

                    // Insert constant into the nested class
                    // Find the closing brace of that nested class
                    var classStart = enumContent.IndexOf(nestedClassPattern, StringComparison.Ordinal);
                    var classBlockStart = enumContent.IndexOf('{', classStart) + 1;
                    var classBlockEnd = enumContent.IndexOf('}', classBlockStart);

                    enumContent = enumContent.Insert(classBlockEnd,
                        "\t\t" + constLine + "\n");
                }

                // Write back
                await File.WriteAllTextAsync(_enumFilePath, enumContent);
            });
        }

    }
}
