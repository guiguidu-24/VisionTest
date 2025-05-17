using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace VSExtension
{
    public class PreviewApiService //TODO: IPreviewApiService 
    {


        public string GetText(BitmapImage bitmapImage)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "C:\\Users\\guill\\Programmation\\dotNET_doc\\POC_Tesseract\\PreviewAPI\\bin\\Debug\\net8.0-windows\\PreviewAPI.exe", //TODO  Import the exe into the project as a resource
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            if (bitmapImage == null)
            {
                throw new ArgumentNullException(nameof(bitmapImage), "BitmapImage cannot be null.");
            }

            // Save the BitmapImage to a temporary file
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
            SaveBitmapImageToFile(bitmapImage, tempFilePath);

            // Send the "ocr" command to the running process
            startInfo.Arguments = $"ocr {tempFilePath}";

            using var process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };


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
