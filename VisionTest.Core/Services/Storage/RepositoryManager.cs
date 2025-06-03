using VisionTest.Core.Models;

namespace VisionTest.Core.Services.Storage
{
    public class RepositoryManager
    {
        private readonly ScreenElementStorageService _screenElementStorageService;
        private readonly string _enumFilePath;

        public RepositoryManager(string projectDirectory)
        {
            _enumFilePath = Path.Combine(projectDirectory, "ScreenElements.cs");
            _screenElementStorageService = new ScreenElementStorageService(projectDirectory);
        }

        /// <summary>
        /// Adds a new screen element to the storage and updates the ScreenElementsEnum.cs file.
        /// </summary>
        /// <param name="screenElement"></param>
        /// <param name="projectDirectory"></param>
        /// <returns></returns>
        public async Task AddAsync(ScreenElement screenElement, string projectDirectory)
        {
            Task saveTask = _screenElementStorageService.SaveAsync(screenElement);

            var names = await _screenElementStorageService.GetAllNamesAsync();

            if (names.Contains(screenElement.Id))
            {
                throw new ArgumentException($"Screen element with ID '{screenElement.Id}' already exists.");
            }

            var addToEnumTask = Task.Run (async () =>
            {
                if (!File.Exists(_enumFilePath))
                {
                    using FileStream fileStream = File.Create(_enumFilePath);
                    using StreamWriter fileWriter = new StreamWriter(fileStream);
                    await fileWriter.WriteLineAsync($"namespace {Path.GetFileName(projectDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))};");
                    await fileWriter.WriteAsync("public static class ScreenElements \n{ \n}");
                }

                using var writer = new StreamWriter(_enumFilePath, append: false);
                string enumContent = await File.ReadAllTextAsync(_enumFilePath);
                
                enumContent = enumContent.Insert(enumContent.LastIndexOf("}") - 1, $"\t{screenElement.Id},\n");
                enumContent = enumContent.Remove(enumContent.LastIndexOf("}"));

                enumContent += $"public const string {screenElement.Id} = \"{screenElement.Id}\";\n}}";
                await writer.WriteAsync(enumContent);
            });

            await Task.WhenAll(saveTask, addToEnumTask);
        }
    
        public async Task<ScreenElement?> GetByIdAsync(string id)
        {
            return await _screenElementStorageService.GetByIdAsync(id);
        }

        public async Task RemoveElement(string id)
        {
            if (await _screenElementStorageService.ExistsAsync(id))
            {
                var deleteElementTask =  _screenElementStorageService.DeleteAsync(id);
                var removeEltFromEnumTask =  RemoveElementFromEnum(id);

                await Task.WhenAll(deleteElementTask, removeEltFromEnumTask);
            }
            else
            {
                throw new ArgumentException($"Screen element with ID '{id}' does not exist.");
            }
        }

        private async Task RemoveElementFromEnum(string id)
        {
            if (File.Exists(_enumFilePath))
            {
                var lines = (await File.ReadAllLinesAsync(_enumFilePath)).ToList();
                lines.RemoveAll(line => line.Contains("public const " + id + " "));
               
                await File.WriteAllLinesAsync(_enumFilePath, lines);
            }
        }
    }
}
