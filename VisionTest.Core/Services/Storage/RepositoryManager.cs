using VisionTest.Core.Models;

namespace VisionTest.Core.Services.Storage
{
    public class RepositoryManager
    {
        private readonly ScreenElementStorageService _screenElementStorageService;
        private readonly string _enumFilePath;
        private readonly IndexationService _indexationService;

        public RepositoryManager(string projectDirectory)
        {
            _enumFilePath = Path.Combine(projectDirectory, "ScreenElements.cs");
            _screenElementStorageService = new ScreenElementStorageService(projectDirectory);
            _indexationService = new IndexationService(_enumFilePath, projectDirectory);
        }

        /// <summary>
        /// Adds a new screen element to the storage and updates the ScreenElementsEnum.cs file.
        /// </summary>
        /// <param name="screenElement"></param>
        /// <returns></returns>
        public async Task AddAsync(ScreenElement screenElement)
        {
            var saveTask = _screenElementStorageService.SaveAsync(screenElement);
            
            var addToEnumTask = _indexationService.AddElementToIndexAsync(screenElement);

            await Task.WhenAll(saveTask, addToEnumTask);
        }

        /// <summary>
        /// Retrieves a screen element by its unique identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ScreenElement?> GetByIdAsync(string id)
        {
            return await _screenElementStorageService.GetByIdAsync(id);
        }

        /// <summary>
        /// Removes a screen element by its unique identifier and updates the ScreenElementsEnum.cs file.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task RemoveElementAsync(string id)
        {
            if (await _screenElementStorageService.ExistsAsync(id))
            {
                var deleteElementTask =  _screenElementStorageService.DeleteAsync(id);
                var removeEltFromEnumTask =  _indexationService.RemoveElementFromIndexAsync(id);

                await Task.WhenAll(deleteElementTask, removeEltFromEnumTask);
            }
            else
            {
                throw new ArgumentException($"Screen element with ID '{id}' does not exist.");
            }
        }

        /// <summary>
        /// Updates the ScreenElement.cs file with all existing screen elements.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateIndexAsync()
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
                    await _indexationService.AddElementToIndexAsync(screenElement);
                }
            }
        }

        /// <summary>
        /// Retrieves all the names of the saved screen elements from the storage directory.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetAllScreenElementNamesAsync()
        {
            return await _screenElementStorageService.GetAllNamesAsync();
        }
    }
}
