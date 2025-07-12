using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json.Linq;
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
            _interopProcess.StandardInput.WriteLine($"add {ProjectService.GetActiveProjectDirectory()} {id} {tempImagePath} -delete");
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

            // Read the JSON response from the process
            string jsonResponse = _interopProcess.StandardOutput.ReadLine();

            try
            {
                File.Delete(tempFilePath);
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"Failed to delete temporary file: {ex.Message}");
                throw;
            }

            // Parse the JSON and extract the textFound field
            if (string.IsNullOrWhiteSpace(jsonResponse))
                throw new InvalidOperationException("No response received from OCR process.");

            try
            {
                var root = JObject.Parse(jsonResponse);
                var response = root["response"];
                if (response == null)
                    throw new InvalidOperationException("Invalid OCR response format: missing 'response' property.");
                var data = response["data"];
                if (data == null)
                    throw new InvalidOperationException("Invalid OCR response format: missing 'data' property.");
                var textFound = data["textFound"];
                if (textFound == null)
                    throw new InvalidOperationException("Invalid OCR response format: missing 'textFound' property.");

                return textFound.Value<string>() ?? string.Empty;
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                throw new InvalidOperationException("Failed to parse OCR JSON response.", ex);
            }
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
