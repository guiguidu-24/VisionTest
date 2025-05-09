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
                    FileName = "E:\\Projects data\\POC_Tesseract\\PreviewAPI\\bin\\Debug\\net8.0-windows\\PreviewAPI.exe", // Replace with the actual console app name
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
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
            SaveBitmapImageToFile(bitmapImage, tempFilePath);

            try
            {
                // Send the "ocr" command to the running process
                _process.StandardInput.WriteLine($"ocr \"{tempFilePath}\"");
                _process.StandardInput.Flush();

                // Read the response from the process
                string header = _process.StandardOutput.ReadLine();

                // Return the response
                if (header.Trim() != ("Extracted Text:"))
                {
                    throw new InvalidOperationException($"Unexpected response from OCR process: {header}");
                }

                return _process.StandardOutput.ReadToEnd()?.Trim();
            }
            finally
            {
                // Delete the temporary file
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        private void SaveBitmapImageToFile(BitmapImage bitmapImage, string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(fileStream);
            }
        }

        public void Dispose()
        {
            _process.StandardInput.WriteLine("exit");
;            _process.WaitForExit();
            _process.Dispose();
        }
    }
}
