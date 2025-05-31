using VisionTest.Core.Models;

namespace VisionTest.Core.Services
{
    public class RepositoryManager
    {
        private readonly ScreenElementStorageService _screenElementStorageService = new ScreenElementStorageService();

        /// <summary>
        /// Adds a new screen element to the storage and updates the ScreenElementsEnum.cs file.
        /// </summary>
        /// <param name="screenElement"></param>
        /// <param name="projectDirectory"></param>
        /// <returns></returns>
        public async Task AddAsync(ScreenElement screenElement, string projectDirectory)
        {
            Task saveTask = _screenElementStorageService.SaveAsync(screenElement, projectDirectory);
            string enumFilePath = Path.Combine(projectDirectory, "ScreenElementsEnum.cs");

            var names = await _screenElementStorageService.GetAllNamesAsync();

            if (names.Contains(screenElement.Id))
            {
                throw new ArgumentException($"Screen element with ID '{screenElement.Id}' already exists.");
            }

            var addToEnumTask = Task.Run (async () =>
            {
                if (!File.Exists(enumFilePath))
                {
                    using FileStream fileStream = File.Create(enumFilePath);
                    using StreamWriter fileWriter = new StreamWriter(fileStream);
                    await fileWriter.WriteLineAsync($"namespace {Path.GetFileName(projectDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))};");
                    await fileWriter.WriteAsync("public enum ScreenElements \n{ \n}");
                }

                using var writer = new StreamWriter(enumFilePath, append: false);
                string enumContent = await File.ReadAllTextAsync(enumFilePath);
                
                enumContent = enumContent.Insert(enumContent.LastIndexOf("}") - 1, $"\t{screenElement.Id},\n");
                enumContent = enumContent.Remove(enumContent.LastIndexOf("}"));

                enumContent += $", {screenElement.Id}\n}}";
                await writer.WriteAsync(enumContent);
            });

            await Task.WhenAll(saveTask, addToEnumTask);
        }
    }
}
