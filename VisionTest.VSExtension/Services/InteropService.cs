using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using VisionTest.VSExtension.Services;

namespace VisionTest.VSExtension
{
    public class InteropService //TODO: IPreviewApiService
    {
        private readonly InteropProcess _interopProcess;

        public InteropService()
        {
            _interopProcess = new InteropProcess();
            _interopProcess.Start();
        }

        public void Add(BitmapImage image, string id)
        {
            var tempImagePath = SaveBitmapImageToTemp(image);
            _interopProcess.StandardInput.WriteLine($"add {ProjectService.GetActiveProjectDirectory()} {id} {SaveBitmapImageToTemp(image)}");
        }

        public void UpdateEnum()
        {
            _interopProcess.StandardInput.WriteLine($"update {ProjectService.GetActiveProjectDirectory()}");
        }


        public string GetText(BitmapImage bitmapImage)
        {
            if (bitmapImage == null)
            {
                throw new ArgumentNullException(nameof(bitmapImage), "BitmapImage cannot be null.");
            }

            // Save the BitmapImage to a temporary file
            string tempFilePath = SaveBitmapImageToTemp(bitmapImage);

            // Send the "ocr" command to the running process
           
            _interopProcess.StandardInput.WriteLine($"ocr {tempFilePath}");
            // Read the response from the process
            string header = _interopProcess.StandardOutput.ReadLine();

            // Return the response
            if (header.Trim() != "Extracted Text:")
            {
                throw new InvalidOperationException($"Unexpected response from OCR process: {header}");
            }

            var output = _interopProcess.StandardOutput.ReadLine().Trim();


            try
            {
                File.Delete(tempFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to delete temporary file: {ex.Message}");
                throw;
            }

            return output;
        }

        private string SaveBitmapImageToTemp(BitmapImage bitmapImage)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");

            // Ensure the directory exists
            string directoryPath = Path.GetDirectoryName(tempFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(fileStream);
            }

            // Verify the file exists
            if (!File.Exists(tempFilePath))
            {
                throw new IOException($"Failed to create the temporary file at {tempFilePath}");
            }

            return tempFilePath;
        }
        


    }
}
