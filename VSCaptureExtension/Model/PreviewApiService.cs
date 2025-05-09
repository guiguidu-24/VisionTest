using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace VSCaptureExtension
{
    internal class PreviewApiService : IDisposable
    {
        private Process _process;

        public PreviewApiService()
        {
            // Start the console app in headless mode
            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "C:\\Users\\guill\\Programmation\\dotNET_doc\\POC_Tesseract\\PreviewAPI\\bin\\Debug\\net8.0-windows\\PreviewAPI.exe", //TODO  Import the exe into the project as a resource
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            _process.Start();
        }

        public string GetText(BitmapImage bitmapImage)
        {
            if (bitmapImage == null)
            {
                throw new ArgumentNullException(nameof(bitmapImage), "BitmapImage cannot be null.");
            }

            // Save the BitmapImage to a temporary file
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"previewFilesUI\\{Guid.NewGuid()}.png");
            SaveBitmapImageToFile(bitmapImage, tempFilePath);


            // Send the "ocr" command to the running process
            _process.StandardInput.WriteLine($"ocr {tempFilePath}");
            _process.StandardInput.Flush();

            
            // Read the response from the process
            string header = _process.StandardOutput.ReadLine();

            // Return the response
            if (header.Trim() != "Extracted Text:")
            {
                throw new InvalidOperationException($"Unexpected response from OCR process: {header}");
            }

            var output = _process.StandardOutput.ReadLine().Trim();

            // Clear any remaining data in the StandardOutput stream
            _process.StandardOutput.ReadLine();

            return output;
        }

        private void SaveBitmapImageToFile(BitmapImage bitmapImage, string filePath)
        {
            // Ensure the directory exists
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(fileStream);
            }

            // Verify the file exists
            if (!File.Exists(filePath))
            {
                throw new IOException($"Failed to create the temporary file at {filePath}");
            }
        }

        public void Dispose()
        {
            // Stop the process
            _process.StandardInput.WriteLine("exit");
            _process.WaitForExit();
            _process.Dispose();

            // Delete the temporary directory
            string tempDirectoryPath = Path.Combine(Path.GetTempPath(), "previewFilesUI");
            if (Directory.Exists(tempDirectoryPath))
            {
                try
                {
                    Directory.Delete(tempDirectoryPath, recursive: true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to delete temporary directory: {ex.Message}");
                }
            }
        }
    }
}
