using NUnit.Framework;
using VisionTest.Core.Models;
using VisionTest.Core.Utils;


namespace VisionTest.Core.Services.Storage
{
    internal class ScreenElementStorageService
    {
        private const string storageDirectoryName = "TestScriptData";
        private readonly string _storageDirectory;// = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestScriptData");

        internal ScreenElementStorageService(string projectDirectory)
        {
            _storageDirectory = Path.Combine(projectDirectory, storageDirectoryName);
        }


        /// <summary>
        /// Deletes a screen element by its unique identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task DeleteAsync(string id)
        {
            await Task.Run(() => File.Delete(Path.Combine(_storageDirectory, $"{id}.png")));
        }

        /// <summary>
        /// Checks whether a screen element with the given ID exists in the storage directory.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task<bool> ExistsAsync(string id)
        {
            return await Task.Run(() => File.Exists(Path.Combine(_storageDirectory, $"{id}.png")));
        }

        /// <summary>
        /// Retrieves all the ids of the saved screen elements from the storage directory and its subdirectories.
        /// Returns the relative path (from _storageDirectory) without extension for each file.
        /// </summary>
        /// <returns></returns>
        internal async Task<IEnumerable<string>> GetAllNamesAsync()
        {
            return await Task.Run(() =>
                Directory
                    .GetFiles(_storageDirectory, "*.png", SearchOption.AllDirectories)
                    .Select(s =>
                    {
                        var relativePath = Path.GetRelativePath(_storageDirectory, s);
                        return Path.ChangeExtension(relativePath, null);
                    })
                    .Where(s => !string.IsNullOrEmpty(s))
            );
        }

        /// <summary>
        /// Retrieves a screen element by its unique identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task<ScreenElement?> GetByIdAsync(string id)
        {
            var element = new ScreenElement() { Id = id };
            var filePath = Path.Combine(_storageDirectory, $"{id}.png");

            if (!File.Exists(filePath))
                return null;

            await Task.Run(() =>
            {
                element.Images.Add(BitmapExtensions.LoadSafelyImage(filePath));
            });

            return element;
        }

        /// <summary>
        /// Saves a new screen element or updates an existing one.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        internal async Task SaveAsync(ScreenElement element)
        {
            await Task.Run(() =>
            {
                // Replace any directory separators in the id with the system's directory separator
                var relativePath = element.Id.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
                string filePath = Path.Combine(_storageDirectory, $"{relativePath}.png");
                string? dir = Path.GetDirectoryName(filePath);

                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                if (element.Images.Count == 1)
                {
                    element.Images[0].Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                }
                else
                {
                    throw new NotImplementedException("Saving multiple images for a single screen element is not implemented yet.");
                }
            });
        }

    }
}
