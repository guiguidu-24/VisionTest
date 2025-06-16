using VisionTest.Core.Models;

namespace VisionTest.Core.Services.Storage
{
    public class RepositoryManager
    {
        private readonly ScreenElementStorageService _screenElementStorageService;
        private readonly string _enumFilePath;
        private readonly string _projectDirectory;

        public RepositoryManager(string projectDirectory)
        {
            _projectDirectory = projectDirectory;
            _enumFilePath = Path.Combine(projectDirectory, "ScreenElements.cs");
            _screenElementStorageService = new ScreenElementStorageService(projectDirectory);
        }

        /// <summary>
        /// Adds a new screen element to the storage and updates the ScreenElementsEnum.cs file.
        /// </summary>
        /// <param name="screenElement"></param>
        /// <returns></returns>
        public async Task AddAsync(ScreenElement screenElement)
        {
            var saveTask = _screenElementStorageService.SaveAsync(screenElement);
            
            var addToEnumTask = AddElementToEnum(screenElement);

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
            else
            {
                throw new FileNotFoundException($"Enum file '{_enumFilePath}' does not exist.");
            }
        }

        private async Task AddElementToEnum(ScreenElement screenElement) //TODO create a class to represent directories if needed
        {
            await Task.Run(async () =>
            {
                if (!File.Exists(_enumFilePath))
                {
                    using FileStream fileStream = File.Create(_enumFilePath);
                    using StreamWriter fileWriter = new StreamWriter(fileStream);
                    await fileWriter.WriteLineAsync($"namespace {Path.GetFileName(_projectDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))};");
                    await fileWriter.WriteAsync("public static class ScreenElements \n{\n}");
                }

                string enumContent = await File.ReadAllTextAsync(_enumFilePath);

                enumContent = enumContent.Insert(enumContent.LastIndexOf("}"), $"\tpublic const string {screenElement.Id} = \"{screenElement.Id}\";\n");

                using var writer = new StreamWriter(_enumFilePath, append: false);
                await writer.WriteAsync(enumContent);
            });
        }

        public async Task UpdateEnumAsync(string projectDirectory)
        {
            IEnumerable<string> screenElementNames = await _screenElementStorageService.GetAllNamesAsync();
            
            if (File.Exists(_enumFilePath))
            {
                File.Delete(_enumFilePath);
            }

            foreach (var name in screenElementNames)
            {
                var screenElement = await _screenElementStorageService.GetByIdAsync(name);
                if (screenElement != null)
                {
                    await AddElementToEnum(screenElement);
                }
            }
        }

        public async Task<IEnumerable<string>> GetAllScreenElementNamesAsync()
        {
            return await _screenElementStorageService.GetAllNamesAsync();
        }
    }
}
