using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using VisionTest.VSExtension.Services;

namespace VisionTest.VSExtension
{
    public class PreviewApiService //TODO: IPreviewApiService TODO Rename
    {


        public string GetText(BitmapImage bitmapImage)
        {
            if (bitmapImage == null)
            {
                throw new ArgumentNullException(nameof(bitmapImage), "BitmapImage cannot be null.");
            }

            // Save the BitmapImage to a temporary file
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");

            SaveBitmapImageToFile(bitmapImage, tempFilePath);

            // Send the "ocr" command to the running process
            using var process = new InteropProcess($"ocr {tempFilePath}");
            process.Start();

            // Read the response from the process
            string header = process.StandardOutput.ReadLine();

            // Return the response
            if (header.Trim() != "Extracted Text:")
            {
                throw new InvalidOperationException($"Unexpected response from OCR process: {header}");
            }

            var output = process.StandardOutput.ReadLine().Trim();

            process.WaitForExit();


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
    }
}
