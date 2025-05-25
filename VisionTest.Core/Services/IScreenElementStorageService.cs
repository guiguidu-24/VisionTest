using VisionTest.Core.Models;

namespace VisionTest.Core.Services
{
    public interface IScreenElementStorageService
    {
        /// <summary>
        /// Saves a new screen element or updates an existing one.
        /// </summary>
        Task SaveAsync(ScreenElement element);

        /// <summary>
        /// Deletes a screen element by its unique identifier.
        /// </summary>
        Task DeleteAsync(string id);

        /// <summary>
        /// Retrieves a screen element by its unique identifier.
        /// </summary>
        Task<ScreenElement?> GetByIdAsync(string id);

        /// <summary>
        /// Retrieves all saved screen elements.
        /// </summary>
        Task<IEnumerable<ScreenElement>> GetAllAsync();

        /// <summary>
        /// Checks whether a screen element with the given ID exists.
        /// </summary>
        Task<bool> ExistsAsync(string id);
    }
}
